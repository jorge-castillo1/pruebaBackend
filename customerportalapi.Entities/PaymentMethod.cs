using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class PaymentMethod
    {
        public string Dni { get; set; }
        public string AccountType { get; set; }
        public int PaymentMethodType { get; set; }
    }
}
