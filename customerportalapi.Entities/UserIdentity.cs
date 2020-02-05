using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserIdentity
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("emails")]
        public List<EmailAccount> Emails { get; set; } = new List<EmailAccount>();      
    }
}
