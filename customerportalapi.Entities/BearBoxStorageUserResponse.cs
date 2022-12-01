using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class BearBoxStorageUserResponse
    {
        public StorageUserData[] data { get; set; }
    }

    public class StorageUserData
    {
        [JsonProperty("locationID")]
        public int LocationID { get; set; }

        [JsonProperty("notifyPinUse")]
        public bool NotifyPinUse { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        [JsonProperty("rentalCustomerID")]
        public string RentalCustomerID { get; set; }

        [JsonProperty("staff")]
        public int Staff { get; set; }

        [JsonProperty("sms")]
        public object Sms { get; set; }

        [JsonProperty("remoteUse")]
        public bool RemoteUse { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("lastPinUse")]
        public object LastPinUse { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
