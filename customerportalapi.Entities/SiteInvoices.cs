using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class SiteInvoices
    {
        public string Name { get; set; }

        public string StoreCode { get; set; }

        public string StoreId { get; set; }


        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
