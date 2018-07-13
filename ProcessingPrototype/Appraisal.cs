using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingPrototype
{
    [Serializable]
    class Appraisal
    {
        public string Lender { get; set; }
        public Int64 ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientState { get; set; }
        public int ClientZip { get; set; }
    }
}
