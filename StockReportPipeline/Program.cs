using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace StockReportPipeline
{
    class Program
    {
        //Constants
        private HttpClient _client;
        readonly string _baseUrl = "https://api.iextrading.com/1.0/";

        TransformBlock<string, CompanyInfo> GetCompanyInfo;
        TransformBlock<string, List<Dividend>> GetDividendReports;
        TransformBlock<string, KeyStats> GetKeyStatInfo;
        TransformBlock<string, List<Interval>> GetIntervalReports;
        TransformBlock<List<Interval>, List<decimal>> GetChangesOverInterval;
        BroadcastBlock<string> broadcastSymbol;
        ActionBlock<Tuple<List<decimal>, List<Dividend>, KeyStats>> GenerateCompleteReport;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Program program = new Program();
            //var company = program.RetrieveCompanyInfo("F");
            //var dividends = program.RetrieveDividendInfo("F");
            //var stats = program.RetrieveKeyStats("F");
            //var intervals = program.RetrieveIntervals("F",30);
            //var changes = program.ConstructIntervalReport(intervals);

            program.StartPipeline();
            Console.ReadKey();
        }

        public Task StartPipeline()
        {
            broadcastSymbol = new BroadcastBlock<string>(symbol => symbol);
            var joinblock = new JoinBlock<List<decimal>, List<Dividend>, KeyStats>(new GroupingDataflowBlockOptions { Greedy = false });

            GetCompanyInfo = new TransformBlock<string, CompanyInfo>(symbol =>
            {
                return RetrieveCompanyInfo(symbol);
            });

            GetDividendReports = new TransformBlock<string, List<Dividend>>(symbol =>
            {
                return RetrieveDividendInfo(symbol);
            });

            GetKeyStatInfo = new TransformBlock<string, KeyStats>(symbol =>
            {
                return RetrieveKeyStats(symbol);
            });

            GetIntervalReports = new TransformBlock<string, List<Interval>>(symbol =>
            {
                return RetrieveIntervals(symbol, 30);
            });

            GetChangesOverInterval = new TransformBlock<List<Interval>, List<decimal>>(intervals =>
            {
                return ConstructIntervalReport(intervals);
            });

            GenerateCompleteReport = new ActionBlock<Tuple<List<decimal>, List<Dividend>, KeyStats>>(tup =>
            {
                Console.WriteLine("Company: {0}, Market Cap: {1}", tup.Item3.companyName, tup.Item3.marketcap);

                foreach(Dividend div in tup.Item2)
                {
                    Console.WriteLine("Dividend Report: {0}", div.PaymentDate);
                }
                foreach(decimal dec in tup.Item1)
                {
                    Console.WriteLine("Percentage Change: {0}", dec);
                }
                //TODO Report Generation Area
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            var buffer = new BufferBlock<string>();
            buffer.LinkTo(broadcastSymbol);

            //Broadcasts the symbol
            broadcastSymbol.LinkTo(GetIntervalReports);
            broadcastSymbol.LinkTo(GetDividendReports);
            broadcastSymbol.LinkTo(GetKeyStatInfo);
            //Second teir parallel 
            GetIntervalReports.LinkTo(GetChangesOverInterval, options);
            //Joins the parallel blocks back together
            GetDividendReports.LinkTo(joinblock.Target2, options);
            GetKeyStatInfo.LinkTo(joinblock.Target3, options);
            GetChangesOverInterval.LinkTo(joinblock.Target1, options);

            joinblock.LinkTo(GenerateCompleteReport, options);

            buffer.Post("F");
            buffer.Post("AGFS");
            buffer.Post("BAC");
            buffer.Post("FCF");


            buffer.Complete();

            GenerateCompleteReport.Completion.Wait();



            //TODO need to finish pipeline and find better implementation

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retreives the Company info and Deserializes it
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public CompanyInfo RetrieveCompanyInfo(string symbol)
        {
            using (_client = new HttpClient())
            {
                _client.BaseAddress = new Uri(_baseUrl);

                var url = "stock/" + symbol + "/company/";
                var response = _client.GetAsync(url).Result;
                if(response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<CompanyInfo>(result);
                }
            }
            return null; 
        }
        /// <summary>
        /// Gets a list of all the Dividend Payout for the year
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public List<Dividend> RetrieveDividendInfo(string symbol)
        {
            using (_client = new HttpClient())
            {
                _client.BaseAddress = new Uri(_baseUrl);
                var url = "stock/" + symbol + "/dividends/ytd";
                var response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<Dividend>>(result);
                }
            }
            return null;
        }
        /// <summary>
        /// Gets all the key stats on a company
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public KeyStats RetrieveKeyStats(string symbol)
        {
            using (_client = new HttpClient())
            {
                _client.BaseAddress = new Uri(_baseUrl);
                var url = "stock/" + symbol + "/stats";
                var response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<KeyStats>(result);
                }
            }
            return null;
        }


        /// <summary>
        /// Gets all the key stats on a company
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public List<Interval> RetrieveIntervals(string symbol, int intervalValue)
        {
            using (_client = new HttpClient())
            {
                _client.BaseAddress = new Uri(_baseUrl);
                var url = "stock/" + symbol + "/chart/ytd?chartInterval=" + intervalValue.ToString();
                var response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<Interval>>(result);
                }
            }
            return null;
        }
        /// <summary>
        /// Generates a list of percentage change over the specified intervals
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public List<decimal> ConstructIntervalReport(List<Interval> intervals)
        {
            List<decimal> changePercentages = new List<decimal>();
            decimal CurrClose = 0;
            decimal LastClose = 0;
            if (intervals != null)
            {
                foreach (Interval interval in intervals)
                {
                    if (intervals.IndexOf(interval) == 0) { LastClose = interval.close; continue; }
                    CurrClose = interval.close;

                    if (LastClose != 0)
                    {
                        var percentageChangeOverInterval = ((CurrClose - LastClose) / LastClose) * 100;
                        changePercentages.Add(percentageChangeOverInterval);
                    }

                    LastClose = CurrClose;
                }
                return changePercentages;
            }
            return new List<decimal>();
        }

        public void GenerateFinalReport()
        {
            
        }
    }
}
