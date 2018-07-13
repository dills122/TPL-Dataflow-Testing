using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingPrototype
{
    [Serializable]
    public class Appraisals
    {
        [JsonProperty(PropertyName = "Appraisals")]
        List<Appraisal> appraisals { get; set; }
    }
}
