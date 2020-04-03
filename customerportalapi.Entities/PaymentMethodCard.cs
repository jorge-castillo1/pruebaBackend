using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodCard : PaymentMethod
    {
        public string ExternalId { get; set; }
        public string Channel { get; set; }
        public string SiteId { get; set; }
        public string Name { get; set; }
        public string Surnames { get; set; }
        public string Url { get; set; }
        public string idCustomer { get; set; }
        public string ContractNumber { get; set; }
        public string SmContractCode { get; set; }
    }
}
