using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class Country
    {
        public string Code { get; set; }

        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        public string NumericCode { get; set; }

        public string PhonePrefix { get; set; }
    }
}
