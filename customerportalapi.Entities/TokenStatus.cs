using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class TokenStatus
    {
        [JsonProperty("username")]
        public string UserName { get; set;}
        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
