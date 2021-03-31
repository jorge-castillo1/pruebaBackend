using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodCRM
    {
        public string Name { get; set; }
        public string StoreId { get; set; }
        public string SMId { get; set; }
        public string DocumentId { get; set; }
        public string PaymentMethodId { get; set; }
        public string Description { get; set; }

    }
}
