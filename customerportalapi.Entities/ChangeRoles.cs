﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace customerportalapi.Entities
{
    public class ChangeRoles
    {
        public string Email { get; set; }

        public string Dni { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }

        public bool Value { get; set; }
    }
}