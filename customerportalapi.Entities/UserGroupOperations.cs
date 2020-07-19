using Newtonsoft.Json;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class UserGroupOperations
    {
        [JsonProperty("Operations")]
        public List<UserGroupOperation> Operations { get; set; } = new List<UserGroupOperation>();
    }
}
