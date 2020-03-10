using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserGroupRemoveOperation
    {
        [JsonProperty("op")]
        public string Operation { get; set; } = "remove";
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}