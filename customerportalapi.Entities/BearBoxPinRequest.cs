using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class BearBoxPinRequest
    {
        [JsonProperty("pin")]
        public string Pin { get; set; }
    }
}
