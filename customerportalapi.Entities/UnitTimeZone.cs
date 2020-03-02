using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UnitTimeZone
    {
        public Unit Unit { get; set; }
        public string TimeZone { get; set; }

        public string StoreCoordinatesLatitude { get; set; }

        public string StoreCoordinatesLongitude { get; set; }

        public string StoreTelephone { get; set; }

        public string StoreName { get; set; }
    }
}
