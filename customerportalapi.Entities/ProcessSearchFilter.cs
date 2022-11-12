using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ProcessSearchFilter
    {
        public string UserName { get; set; }
        public int? ProcessType { get; set; }
        public string SmContractCode { get; set; }
        public string DocumentId { get; set; }
        public string CardExternalId { get; set; }
        public int? ProcessStatus { get; set; }
        public string PayExternalId { get; set; }
    }
}
