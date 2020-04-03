using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodCardConfirmationToken
    {
        public string ExternalId { get; set; }
        public string Channel { get; set; }
        public bool Confirmed { get; set; }
    }
}
