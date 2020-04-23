using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodPayInvoice
    {
        public string ExternalId { get; set; }
        public string Channel { get; set; }
        public string SiteId { get; set; }
        public string IdCustomer { get; set; }
        public string Token { get; set; }
        public string Amount { get; set; }
        public string Ourref { get; set; }
        public string Documentid { get; set; }
    }
}
