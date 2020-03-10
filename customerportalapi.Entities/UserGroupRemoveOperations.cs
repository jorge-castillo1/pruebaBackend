using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserGroupRemoveOperations
    {
        [JsonProperty("Operations")]
        public List<UserGroupRemoveOperation> Operations { get; set; } = new List<UserGroupRemoveOperation>();
    }
}