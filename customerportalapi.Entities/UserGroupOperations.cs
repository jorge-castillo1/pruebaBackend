using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserGroupOperations
    {
        [JsonProperty("Operations")]
        public List<UserGroupOperation> Operations { get; set; } = new List<UserGroupOperation>();
    }
}
