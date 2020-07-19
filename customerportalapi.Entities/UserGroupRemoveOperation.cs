using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class UserGroupRemoveOperation
    {
        [JsonProperty("op")]
        public string Operation { get; set; } = "remove";
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}