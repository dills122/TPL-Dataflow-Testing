using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineServicePrototype.Models
{
   public class Response
    {
        public string Status { get; set; }
        public string Type { get; set; }
        public int ProccessedUnits { get; set;}
    }
}
