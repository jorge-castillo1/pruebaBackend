using Newtonsoft.Json;

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
