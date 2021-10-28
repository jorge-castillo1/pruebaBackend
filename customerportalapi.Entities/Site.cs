using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Site
    {
        public string Name { get; set; }

        public string Telephone { get; set; }

        public string CoordinatesLatitude { get; set; }

        public string CoordinatesLongitude { get; set; }

        public string EmailAddress1 { get; set; }

        public string StoreCode { get; set; }

        public string StoreId { get; set; }

        public List<Contract> Contracts { get; } = new List<Contract>();

        public string MailType { get; set; }
    }
}
