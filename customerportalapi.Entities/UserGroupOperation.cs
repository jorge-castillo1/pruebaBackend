using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class UserGroupOperation
    {
        [JsonProperty("op")]
        public string Operation { get; set; }
        [JsonProperty("value")]
        public UserGroupOperationValue Value { get; set; }
    }
}
