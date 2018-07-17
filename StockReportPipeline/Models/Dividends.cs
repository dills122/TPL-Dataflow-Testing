using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockReportPipeline
{
    [Serializable]
    public class Dividends
    {
        public List<Dividend> dividends { get; set; }
    }
    [Serializable]
    public class Dividend
    {
        [JsonProperty(PropertyName = "exDate")]
        public DateTime ExDate { get; set; }
        [JsonProperty(PropertyName = "paymentDate")]
        public DateTime PaymentDate { get; set; }
        [JsonProperty(PropertyName = "recordDate")]
        public DateTime RecordDate { get; set; }
        [JsonProperty(PropertyName = "declaredDate")]
        public DateTime DeclaredDate { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "qualified")]
        public string Qualified { get; set; }
    }
}
