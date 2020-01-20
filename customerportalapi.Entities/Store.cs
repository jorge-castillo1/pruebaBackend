using System;

namespace customerportalapi.Entities
{
    public class Store
    {
        public Guid StoreId { get; set; }

        public string StoreName { get; set; }

        public string StoreCode { get; set; }

        public string Telephone { get; set; }

        public string FullAddress { get; set; }

        public string OpeningDaysFirst { get; set; }

        public string OpeningDaysLast { get; set; }

        public string TimeZone { get; set; }

        public string OpeningHoursFrom { get; set; }

        public string OpeningHoursTo { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string EmailAddress1 { get; set; }

        public string EmailAddress2 { get; set; }

        public string MapLink { get; set; }

        public string CoordinatesLatitude { get; set; }

        public string CoordinatesLongitude { get; set; }
    }
}
