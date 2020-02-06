using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Group
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("members")]
        public List<UserGroupMember> Members { get; set; }
    }
}
