using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class Country
    {
        public string Code { get; set; }

        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }
    }
}
