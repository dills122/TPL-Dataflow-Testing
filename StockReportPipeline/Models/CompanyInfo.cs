using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockReportPipeline
{
    [Serializable]
    public class CompanyInfo
    {
        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "companyName")]
        public string CompanyName { get; set; }
        [JsonProperty(PropertyName = "exchange")]
        public string Exchange{ get; set; }
        [JsonProperty(PropertyName = "industry")]
        public string Industry{ get; set; }
        [JsonProperty(PropertyName = "website")]
        public string Website{ get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "CEO")]
        public string Ceo { get; set; }
        [JsonProperty(PropertyName = "issueType")]
        public string IssueType{ get; set; }
        [JsonProperty(PropertyName = "sector")]
        public string Sector{ get; set; }
    }
}
