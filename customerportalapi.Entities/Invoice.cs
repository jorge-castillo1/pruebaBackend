using System;

namespace customerportalapi.Entities
{
    public class Invoice
    {
        public DateTime DocumentDate { get; set; }
        public string DocumentType { get; set; }
        public string UnitDescription { get; set; }
        public string OurReference { get; set; }
        public decimal Amount { get; set; }
        public decimal OutStanding { get; set; }
        public string SiteID { get; set; }
        public string DocumentId { get; set; }
        public string ContractId { get; set; }
    }
}
