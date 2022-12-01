using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class BearBoxPinResponse
    {
        public PinData[] data { get; set; }
    }

    public class PinData
    {
        [JsonProperty("qr")]
        public bool Qr { get; set; }

        [JsonProperty("pin")]
        public int Pin { get; set; }

        [JsonProperty("unitID")]
        public int UnitID { get; set; }

        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
