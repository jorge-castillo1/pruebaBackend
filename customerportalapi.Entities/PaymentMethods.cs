using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class PaymentMethods
    {
        public string Name { get; set; }
        public string StoreId { get; set; }
        public string SMId { get; set; }
        public string PaymentMethodId { get; set; }
        public string Description { get; set; }
        public bool? CardPayment { get; set; }
        public bool? BankAccountPayment { get; set; }
    }

    public class PaymentMethodsList
    {
        public List<PaymentMethods> PaymentMethods { get; set; }
    }
}