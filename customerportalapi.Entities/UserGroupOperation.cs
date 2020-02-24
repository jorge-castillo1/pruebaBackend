using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UserGroupOperation
    {
        [JsonProperty("op")]
        public string Operation { get; set; }
        [JsonProperty("value")]
        public UserGroupOperationValue Value { get; set; }
    }
}
