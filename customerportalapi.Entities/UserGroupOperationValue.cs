using Newtonsoft.Json;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class UserGroupOperationValue
    {
        [JsonProperty("members")]
        public List<UserGroupMember> Members { get; set; }
    }
}
