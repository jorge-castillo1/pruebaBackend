using System;
using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class Language
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string IsoCode { get; set; }

        public string StatusCode { get; set; }
    }
}
