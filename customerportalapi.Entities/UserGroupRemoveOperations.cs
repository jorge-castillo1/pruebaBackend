using Newtonsoft.Json;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class UserGroupRemoveOperations
    {
        [JsonProperty("Operations")]
        public List<UserGroupRemoveOperation> Operations { get; set; } = new List<UserGroupRemoveOperation>();
    }
}