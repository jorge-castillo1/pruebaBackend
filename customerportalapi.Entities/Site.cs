using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Site
    {
        public string Name { get; set; }

        public string Telephone { get; set; }

        public string CoordinatesLatitude { get; set; }

        public string CoordinatesLongitude { get; set; }

        public List<Contract> Contracts { get; } = new List<Contract>();
    }
}
