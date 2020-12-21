using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodUpdateCardData
    {
        public string ExternalId { get; set; }
        public string Channel { get; set; }
        public string SiteId { get; set; }
        public string IdCustomer { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string AccountType { get; set; }
        public string Dni { get; set; }
        public string SmContractCode { get; set; }
        public string ContractNumber { get; set; }
        public string Language { get; set; }

    }
}
