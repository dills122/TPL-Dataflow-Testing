using Newtonsoft.Json;
using StockReportPipeline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;

namespace StockReportPipeline
{
   public class Pipeline
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
        TransformBlock<Tuple<List<decimal>, List<Dividend>, KeyStats>, string> GenerateXmlString;
        ActionBlock<string> GenerateCompleteReport;

        public Pipeline()
        {
            
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

            GenerateXmlString = new TransformBlock<Tuple<List<decimal>, List<Dividend>, KeyStats>, string>(tup =>
            {
                var ReportObj = new Report
                {
                    changeIntervals = tup.Item1,
                    dividends = tup.Item2,
                    keyStats = tup.Item3
                };

                XmlSerializer ser = new XmlSerializer(typeof(Report));
                var stringWriter = new StringWriter();
                ser.Serialize(stringWriter, ReportObj);
                return stringWriter.ToString();

            });

            GenerateCompleteReport = new ActionBlock<string>(xml =>
            {
                var str = Path.GetRandomFileName().Replace(".", "") + ".xml";
                File.WriteAllText(str, xml);
                Console.WriteLine("Finished File");
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

            joinblock.LinkTo(GenerateXmlString, options);
            GenerateXmlString.LinkTo(GenerateCompleteReport, options);

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
                if (response.IsSuccessStatusCode)
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
            return new List<Dividend>();
        }
        /// <summary>
        /// Gets all the key stats on a company
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public KeyStats RetrieveKeyStats(string symbol)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            using (_client = new HttpClient())
            {
                _client.BaseAddress = new Uri(_baseUrl);
                var url = "stock/" + symbol + "/stats";
                var response = _client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<KeyStats>(result,settings);
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
