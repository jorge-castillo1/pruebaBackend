using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class SignatureStatusMetadata
    {
        public string ContractNumber { get; set; }

        public string BankAccountOrderNumber { get; set; }

        public string BankAccountName { get; set; }

        public int DocumentType { get; set; }

        public string SmContractCode { get; set; }
    }
}
