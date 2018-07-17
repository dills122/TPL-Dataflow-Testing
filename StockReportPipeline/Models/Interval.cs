using System;
using System.Collections.Generic;
using System.Text;

namespace StockReportPipeline
{
    [Serializable]
    public class Interval
    {
        public string date { get; set; }
        public string minute { get; set; }
        public string label { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal change { get; set; }
        public decimal changePercent { get; set; }
        public decimal average { get; set; }
        public Int64 volume { get; set; }
        public long notional { get; set; }
        public Int64 numberOfTrades { get; set; }
        public decimal marketHigh { get; set; }
        public decimal marketLow { get; set; }
        public decimal marketAverage { get; set; }
        public Int64 marketVolume { get; set; }
        public decimal marketNotional { get; set; }
        public Int64 marketNumberOfTrades { get; set; }
        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal marketOpen { get; set; }
        public decimal marketClose { get; set; }
        public decimal changeOverTime { get; set; }
        public decimal marketChangeOverTime { get; set; }
    }
}
