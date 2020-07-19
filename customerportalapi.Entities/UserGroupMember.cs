using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class UserGroupMember
    {
        [JsonProperty("display")]
        public string Display { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
