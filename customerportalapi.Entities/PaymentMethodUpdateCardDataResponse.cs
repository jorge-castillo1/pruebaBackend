using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodUpdateCardResponse
    {
        public string externalid { get; set; }
        public string IdCustomer { get; set; }
        public string siteid { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string cardholder { get; set; }
        public string expirydate { get; set; }
        public string typecard { get; set; }
        public string cardnumber { get; set; }

    }
}