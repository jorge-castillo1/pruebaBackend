using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodCardConfirmationResponse
    {
        public string ExternalId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
