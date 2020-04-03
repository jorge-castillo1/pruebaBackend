using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Invoice
    {
        public DateTime DocumentDate { get; set; }
        public string UnitDescription { get; set; }
        public string OurReference { get; set; }
        public decimal Amount { get; set; }
        public decimal OutStanding { get; set; }
        public string SiteID { get; set; }
    }
}
