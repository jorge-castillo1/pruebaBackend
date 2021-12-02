using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class FullContractList
    {
        //public string odatacontext { get; set; }

        [JsonProperty("value")]
        public List<FullContract> Contracts { get; set; }
    }

    public class FullContract
    {
        public string odataetag { get; set; }
        public string iav_contractid { get; set; }
        public string iav_name { get; set; }
        public string iav_smcontractcode { get; set; }
        public string _iav_storeid_value { get; set; }
        public string _iav_unitid_value { get; set; }
        public string _transactioncurrencyid_value { get; set; }
        public float iav_price { get; set; }
        public float iav_vat { get; set; }
        public float? iav_reservationfee { get; set; }
        public string iav_promotions { get; set; }
        public DateTime iav_contractdate { get; set; }
        public DateTime iav_firstpaymentdate { get; set; }
        public float iav_firstpaymentmoney { get; set; }
        public string _iav_paymentmethodid_value { get; set; }
        public bool iav_paid { get; set; }
        public string _iav_opportunityid_value { get; set; }
        public string _iav_customerid_value { get; set; }
        public int iav_opportunitytype { get; set; }
        public string _blue_closedby_value { get; set; }
        public string new_contacturl { get; set; }
        public string new_signaturestatus { get; set; }
        public Store iav_storeid { get; set; }
        public Unit iav_unitid { get; set; }
        public Iav_Opportunityid iav_opportunityid { get; set; }
        public Iav_Customerid iav_customerid { get; set; }
    }

    public class Iav_Opportunityid
    {
        public string opportunityid { get; set; }
        public DateTime iav_expectedmovein { get; set; }
    }

    public class Iav_Customerid
    {
        public int iav_documenttype { get; set; }
        public string iav_dni { get; set; }
        public string emailaddress1 { get; set; }
        public string emailaddress2 { get; set; }
        public string address1_telephone1 { get; set; }
        public string telephone1 { get; set; }
        public object telephone2 { get; set; }
        public string address1_telephone2 { get; set; }
        public string address1_composite { get; set; }
        public string address1_line1 { get; set; }
        public string address1_line2 { get; set; }
        public string address1_line3 { get; set; }
        public string address1_city { get; set; }
        public string address1_postalcode { get; set; }
        public string address1_stateorprovince { get; set; }
        public string address1_country { get; set; }
        public bool? blue_updatewebportal { get; set; }
        public string blue_customertypestring { get; set; }
    }
}