using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class ContractStatusRequest
    {
        [JsonProperty("signature_id")]
        public string SignatureId { get; set; }

        //[JsonProperty("id")]
        //public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
