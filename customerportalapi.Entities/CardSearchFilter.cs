using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class CardSearchFilter
    {
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string SmContractCode { get; set; }
        public string ContractNumber { get; set; }
        public bool Current { get; set; }

    }
}
