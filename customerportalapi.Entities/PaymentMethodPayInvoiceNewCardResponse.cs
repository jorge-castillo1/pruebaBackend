using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodPayInvoiceNewCardResponse
    {
        public string ExternalId { get; set; }
        public string SiteId { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        
    }
}
