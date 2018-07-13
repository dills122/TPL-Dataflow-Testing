using System;
using System.Collections.Generic;
using System.Text;

namespace DataFlowPipelinePrototype
{
    [Serializable]
    class Appraisal
    {
        public string Lender { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientState { get; set; }
        public int ClientZip { get; set; }
    }
}
