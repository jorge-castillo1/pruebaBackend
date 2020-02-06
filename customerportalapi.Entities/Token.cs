using System;
using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccesToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public string Scope { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

    }
}
