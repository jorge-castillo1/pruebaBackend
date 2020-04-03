using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodCardSignature : PaymentMethod
    {
        public string ContractNumber { get; set; }
        public string SmContractCode { get; set; }
        public string UnitNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CardHolderCif { get; set; }
        public string CardHolderAddress { get; set; }
        public string CardHolderPostalCode { get; set; }
        public string CardHolderCity { get; set; }
        public string SiteId { get; set; }
        public string ExternalId { get; set;}
        public string Username { get; set; }
        public string CompanyLegalName { get; set; }
        public string StoreCity { get; set; }
    }
}
