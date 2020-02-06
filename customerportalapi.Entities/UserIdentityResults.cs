using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserIdentityResults
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }
        [JsonProperty("Resources")]
        public List<UserIdentity> Users { get; set; } = new List<UserIdentity>();
    }
}
