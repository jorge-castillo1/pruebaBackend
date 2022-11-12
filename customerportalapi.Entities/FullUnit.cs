using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class FullUnit
    {
        //[JsonProperty("productid")]
        //public Guid UnitId { get; set; }

        [JsonProperty("name")]
        public string UnitName { get; set; }

        //[JsonProperty("productnumber")]
        //public string UnitNumber { get; set; }

        [JsonProperty("iav_smunitid")]
        public string SmUnitId { get; set; }

        //[JsonProperty("_iav_storeid_value@OData.Community.Display.V1.FormattedValue")]
        //public string Store { get; set; }

        //[JsonProperty("iav_size@OData.Community.Display.V1.FormattedValue")]
        [JsonProperty("iav_size")]
        public string Size { get; set; }

        [JsonProperty("iav_unitcategorytext")]
        public string UnitCategory { get; set; }

        [JsonProperty("iav_subtype")]
        public string Subtype { get; set; }

        //[JsonProperty("statuscode@OData.Community.Display.V1.FormattedValue")]
        //public string StatusReason { get; set; }

        //[JsonProperty("_transactioncurrencyid_value@OData.Community.Display.V1.FormattedValue")]
        //public string Currency { get; set; }

        //[JsonProperty("iav_weekrate@OData.Community.Display.V1.FormattedValue")]
        //public string WeekRate { get; set; }

        //[JsonProperty("iav_monthrate@OData.Community.Display.V1.FormattedValue")]
        //public string MonthRate { get; set; }

        //[JsonProperty("iav_height@OData.Community.Display.V1.FormattedValue")]
        [JsonProperty("iav_height")]
        public string Height { get; set; }

        //[JsonProperty("iav_width@OData.Community.Display.V1.FormattedValue")]
        [JsonProperty("iav_width")]
        public string Width { get; set; }

        //[JsonProperty("iav_depth@OData.Community.Display.V1.FormattedValue")]
        [JsonProperty("iav_depth")]
        public string Depth { get; set; }

        //[JsonProperty("new_deposit@OData.Community.Display.V1.FormattedValue")]
        //public string Deposit { get; set; }

        [JsonProperty("blue_colour")]
        public string Colour { get; set; }

        [JsonProperty("blue_corridor")]
        public string Corridor { get; set; }

        [JsonProperty("blue_exceptions")]
        public string Exceptions { get; set; }

        [JsonProperty("blue_floor")]
        public string Floor { get; set; }

        [JsonProperty("blue_zone")]
        public string Zone { get; set; }

    }

    public class UnitList
    {
        [JsonProperty("value")]
        public List<Unit> Units { get; set; }
    }
}