using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserGroupOperationValue
    {
        [JsonProperty("members")]
        public List<UserGroupMember> Members { get; set; }
    }
}
