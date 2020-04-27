using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class MakePayment
    {
        public string CustomerId { get; set; }
        public string SiteId { get; set; }
        public string DocumentId { get; set; }
        public string PayMethod { get; set; }
        public decimal PayAmount { get; set; }
        public string PayRef { get; set; }
    }
}
