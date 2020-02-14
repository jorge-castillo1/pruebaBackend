using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ProfilePermissions
    {
        [JsonProperty("documentnumber")]
        public string DocumentNumber { get; set; }

        [JsonProperty("supercontact")]
        public bool CanManageContacts { get; set; }
        [JsonProperty("admincontact")]
        public bool CanManageAccounts { get; set; }
    }
}
