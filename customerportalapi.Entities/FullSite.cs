using customerportalapi.Entities.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class FullSite
    {
        [JsonProperty("iav_storeid")]
        public Guid StoreId { get; set; }

        [JsonProperty("new_tradename")]
        public string StoreName { get; set; }

        [JsonProperty("iav_storecode")]
        public string StoreCode { get; set; }

        [JsonProperty("iav_telephone")]
        public string Telephone { get; set; }

        [JsonProperty("iav_address")]
        public string FullAddress { get; set; }

        [JsonProperty("iav_openingdaysstartday@OData.Community.Display.V1.FormattedValue")]
        public string OpeningDaysFirst { get; set; }

        [JsonProperty("iav_openingdayslastday@OData.Community.Display.V1.FormattedValue")]
        public string OpeningDaysLast { get; set; }

        [JsonProperty("iav_storetimezone@OData.Community.Display.V1.FormattedValue")]
        public string TimeZone { get; set; }

        [JsonProperty("iav_openinghoursstart@OData.Community.Display.V1.FormattedValue")]
        public string OpeningHoursFrom { get; set; }

        [JsonProperty("iav_openinghoursend@OData.Community.Display.V1.FormattedValue")]
        public string OpeningHoursTo { get; set; }

        [JsonProperty("new_openinghoursstartweekend@OData.Community.Display.V1.FormattedValue")]
        public string OpeningHoursFromSaturday { get; set; }

        [JsonProperty("new_openinghoursendweekend@OData.Community.Display.V1.FormattedValue")]
        public string OpeningHoursToSaturday { get; set; }

        [JsonProperty("_iav_cityid_value@OData.Community.Display.V1.FormattedValue")]
        public string City { get; set; }

        [JsonProperty("iav_emailaddress")]
        public string EmailAddress1 { get; set; }

        [JsonProperty("emailaddress")]
        public string EmailAddress2 { get; set; }

        [JsonProperty("iav_maplink")]
        public string MapLink { get; set; }

        [JsonProperty("blue_companycountry")]
        public string Country { get; set; }

        [JsonProperty("blue_iso")]
        public string CountryCode { get; set; }

        [JsonProperty("blue_countryiso")]
        public string CountryAndCode { get; set; }

        [JsonProperty("blue_coordinaten")]
        public string CoordinatesLatitude { get; set; }

        [JsonProperty("blue_coordinatee")]
        public string CoordinatesLongitude { get; set; }

        private string _accessType { get; set; }

        [JsonProperty("blue_accesstype")]
        public string AccessType
        {
            get { return _accessType; }
            set { _accessType = SiteAccessTypeHelper.GetSiteAccessTypeOptionSet(value); }
        }

        [JsonProperty("_blue_defaultlanguage_value@OData.Community.Display.V1.FormattedValue")]
        public string DefaultLanguage { get; set; }

        [JsonProperty("blue_companyname")]
        public string CompanyName { get; set; }

        [JsonProperty("blue_companycif")]
        public string CompanyCif { get; set; }

        [JsonProperty("blue_companysocialadress")]
        public string CompanySocialAddress { get; set; }

        [JsonProperty("new_documentrepository")]
        public string DocumentRepositoryUrl { get; set; }

        [JsonProperty("blue_storeimage")]
        public string StoreImage { get; set; }

        [JsonProperty("blue_mailtype")]
        public string MailType { get; set; }
    }

    public class SiteList
    {
        [JsonProperty("value")]
        public List<Site> Sites { get; set; }
    }
}
