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
        public List<string> Emails { get; set; } = new List<string>();
        [JsonProperty("preferedLanguage")]
        public string Language { get; set; }
        [JsonProperty("userType")]
        public string UserType { get; set; }
        [JsonProperty("externalId")]
        public string CardId { get; set; } 
         [JsonProperty("displayName")]
        public string DisplayName { get; set; }  
    }
}
