using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class EmailAccount
    {
        [JsonProperty("primary")]
        public bool Primary { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
