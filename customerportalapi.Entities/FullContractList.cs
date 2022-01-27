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

        [JsonProperty("_iav_storeid_value@OData.Community.Display.V1.FormattedValue")]
        public string _iav_store { get; set; }
        public string _iav_storeid_value { get; set; }

        [JsonProperty("_iav_unitid_value@OData.Community.Display.V1.FormattedValue")]
        public string _iav_unit { get; set; }
        public string _iav_unitid_value { get; set; }

        [JsonProperty("_transactioncurrencyid_value@OData.Community.Display.V1.FormattedValue")]
        public string _transactioncurrency { get; set; }
        public string _transactioncurrencyid_value { get; set; }

        [JsonProperty("iav_price@OData.Community.Display.V1.FormattedValue")]
        public string iav_price_currency { get; set; }
        public float iav_price { get; set; }

        [JsonProperty("iav_vat@OData.Community.Display.V1.FormattedValue")]
        public string iav_vat_currency { get; set; }
        public float iav_vat { get; set; }

        public float? iav_reservationfee { get; set; }
        public string iav_promotions { get; set; }

        [JsonProperty("iav_contractdate@OData.Community.Display.V1.FormattedValue")]
        public string iav_contractdate_formatted { get; set; }
        public DateTime iav_contractdate { get; set; }

        [JsonProperty("iav_firstpaymentdate@OData.Community.Display.V1.FormattedValue")]
        public string iav_firstpaymentdate_formatted { get; set; }
        public DateTime iav_firstpaymentdate { get; set; }

        [JsonProperty("iav_firstpaymentmoney@OData.Community.Display.V1.FormattedValue")]
        public string iav_firstpaymentmoney_currency { get; set; }
        public float iav_firstpaymentmoney { get; set; }

        [JsonProperty("_iav_paymentmethodid_value@OData.Community.Display.V1.FormattedValue")]
        public string _iav_paymentmethod { get; set; }
        public string _iav_paymentmethodid_value { get; set; }

        [JsonProperty("iav_paid@OData.Community.Display.V1.FormattedValue")]
        public string iav_paid_string { get; set; }
        public bool iav_paid { get; set; }

        [JsonProperty("_iav_opportunityid_value@OData.Community.Display.V1.FormattedValue")]
        public string _iav_opportunity { get; set; }
        public string _iav_opportunityid_value { get; set; }

        [JsonProperty("_iav_customerid_value@OData.Community.Display.V1.FormattedValue")]
        public string _iav_customer { get; set; }
        public string _iav_customerid_value { get; set; }

        [JsonProperty("iav_opportunitytype@OData.Community.Display.V1.FormattedValue")]
        public string iav_opportunitytype_string { get; set; }
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



/*
 * // EXAMPLE 
 {
	"@odata.context": "https://bluebasetest.bluespace.es/BLUESPACEPRE/api/data/v8.1/$metadata#iav_contracts(iav_contractid,iav_name,iav_smcontractcode,_iav_storeid_value,_iav_unitid_value,_transactioncurrencyid_value,iav_price,iav_vat,iav_reservationfee,iav_promotions,iav_contractdate,iav_firstpaymentdate,iav_firstpaymentmoney,_iav_paymentmethodid_value,iav_paid,_iav_opportunityid_value,_iav_customerid_value,iav_opportunitytype,_blue_closedby_value,new_contacturl,new_signaturestatus,iav_storeid,iav_unitid,iav_opportunityid,iav_customerid,iav_unitid(name,iav_smunitid,iav_size,iav_unitcategorytext,iav_subtype,price,iav_height,iav_width,iav_depth),iav_opportunityid(opportunityid,iav_expectedmovein),iav_customerid(iav_documenttype,iav_dni,emailaddress1,emailaddress2,address1_telephone1,telephone1,telephone2,address1_telephone1,address1_telephone2,address1_composite,address1_line1,address1_line2,address1_line3,address1_city,address1_postalcode,address1_stateorprovince,address1_country,blue_updatewebportal,blue_customertypestring))",
	"value": [
		{
			"@odata.etag": "W/\"481701571\"",
			"iav_contractid": "25259292-ecab-ea11-9412-00505691c4bd",
			"iav_name": "168598",
			"iav_smcontractcode": "RI1DKBXX311020170032",
			"_iav_storeid_value@OData.Community.Display.V1.FormattedValue": "Hospitalet",
			"_iav_storeid_value": "6ee7e31d-38eb-e611-940d-00505691c4bd",
			"_iav_unitid_value@OData.Community.Display.V1.FormattedValue": "2733",
			"_iav_unitid_value": "381761e7-95eb-e611-940d-00505691c4bd",
			"_transactioncurrencyid_value@OData.Community.Display.V1.FormattedValue": "Euro",
			"_transactioncurrencyid_value": "444d83e5-a8f5-e511-93fe-00505691c4bd",
			"iav_price@OData.Community.Display.V1.FormattedValue": "€50.00",
			"iav_price": 50.0,
			"iav_vat@OData.Community.Display.V1.FormattedValue": "€10.50",
			"iav_vat": 10.5,
			"iav_reservationfee": null,
			"iav_promotions": "1€ quote",
			"iav_contractdate@OData.Community.Display.V1.FormattedValue": "31/10/2017",
			"iav_contractdate": "2017-10-30T23:00:00Z",			
			"iav_firstpaymentdate@OData.Community.Display.V1.FormattedValue": "31/10/2017",
			"iav_firstpaymentdate": "2017-10-30T23:00:00Z",
			"iav_firstpaymentmoney@OData.Community.Display.V1.FormattedValue": "€10.00",
			"iav_firstpaymentmoney": 10.0,
			"_iav_paymentmethodid_value@OData.Community.Display.V1.FormattedValue": "Recibo domiciliado",
			"_iav_paymentmethodid_value": "983a75ad-38eb-e611-940d-00505691c4bd",
			"iav_paid@OData.Community.Display.V1.FormattedValue": "No",
			"iav_paid": false,
			"_iav_opportunityid_value@OData.Community.Display.V1.FormattedValue": "   01/07/2021 ",
			"_iav_opportunityid_value": "2734e058-09ea-eb11-9455-00505691eca1",
			"_iav_customerid_value@OData.Community.Display.V1.FormattedValue": "Perez Gil",
			"_iav_customerid_value": "e87349a6-be84-e811-9412-00505691c4bd",
			"iav_opportunitytype@OData.Community.Display.V1.FormattedValue": "Unit",
			"iav_opportunitytype": 808340001,
			"_blue_closedby_value": null,
			"new_contacturl": "https://bluespaceselfstorage.sharepoint.com/sites/Stores-PRE/bellvitge/Documentos compartidos/Residential/47912703T",
			"new_signaturestatus": null,
			"iav_storeid": {
				"@odata.etag": "W/\"481559426\"",
				"blue_storeimage": "https://prewebportalsa.blob.core.windows.net/stores-images/720cdf33-22fb-48bf-bb39-c893fa5ba49b.png",
				"emailaddress": "hospitalet@bluespace.es",
				"_ownerid_value": "15d8bb17-a8f5-e511-93fe-00505691c4bd",
				"_blue_defaultlanguage_value": "2f7a4149-d3de-e511-80bf-00155d018a4f",
				"timezoneruleversionnumber": null,
				"iav_storetimezone": 808340000,
				"iav_openinghoursend": 808340040,
				"blue_iso": "ES",
				"blue_accesstype": 802260001,
				"blue_companycountry": "ESPAÑA",
				"blue_openinghourstartsunday": null,
				"blue_aoplink": 802260000,
				"_createdby_value": "15d8bb17-a8f5-e511-93fe-00505691c4bd",
				"blue_companyname": "Blue Self-Storage, S.L.",
				"blue_coordinaten": 41.351764,
				"_blue_businessname_value": "1d09dc2c-b320-ea11-9412-00505691c4bd",
				"modifiedon": "2022-01-11T14:33:04Z",
				"blue_companycityregistered": "Barcelona",
				"iav_comments": null,
				"blue_openinghourendsunday": null,
				"blue_countryiso": "ES - ESPAÑA",
				"blue_removedeposit": null,
				"statuscode": 1,
				"statecode": 0,
				"iav_tempdatetime": null,
				"_owningbusinessunit_value": "aed96e11-a8f5-e511-93fe-00505691c4bd",
				"_iav_cityid_value": "b02fc244-40e4-e511-80bf-00155d018a4f",
				"iav_occupancy": null,
				"blue_storename": "bellvitge",
				"blue_companycif": "B62922604",
				"importsequencenumber": null,
				"iav_openingdayslastday": 808340019,
				"new_openinghoursendweekend": 808340028,
				"_iav_timezoneid_value": "2db886a1-4805-e611-80c5-00155d018a4f",
				"blue_coordinatee": 2.105134,
				"new_mktstorecode": "1",
				"iav_telephone": "932637960",
				"new_tradename": "Bellvitge",
				"utcconversiontimezonecode": null,
				"_modifiedonbehalfby_value": null,
				"_modifiedby_value": "e774f47c-4e9b-ea11-9412-00505691c4bd",
				"blue_mailtype": null,
				"iav_storecode": "PT1IISXX150420050000",
				"blue_companyfolio": "42",
				"iav_emailaddress": "hospitalet@bluespace.es",
				"new_esignature": false,
				"new_urldestino": "561",
				"_iav_associatedteamid_value": "5b31e9c2-9a16-e611-80c5-00155d018a4f",
				"iav_name": "Hospitalet",
				"blue_companytomo": "34869",
				"iav_address": "Travesia industrial 95",
				"iav_storeid": "6ee7e31d-38eb-e611-940d-00505691c4bd",
				"blue_recordurl": null,
				"iav_strategy": null,
				"blue_3dsecure": true,
				"createdon": "2017-02-05T00:14:54Z",
				"versionnumber": 481559426,
				"_owningteam_value": null,
				"_createdonbehalfby_value": null,
				"_iav_ownerid_value": null,
				"_iav_nearestcenter_value": null,
				"iav_maplink": "https://goo.gl/maps/7ULrVaWD5KM2",
				"new_documentrepository": "https://bluespaceselfstorage.sharepoint.com/sites/stores-pre/bellvitge",
				"iav_mktstorecode": null,
				"blue_companysocialadress": "C/ Bravo Murillo, 194, 28020, Madrid",
				"overriddencreatedon": null,
				"iav_openinghoursstart": 808340020,
				"blue_bluemovesales": true,
				"blue_companyhoja": "B252799",
				"_owninguser_value": "15d8bb17-a8f5-e511-93fe-00505691c4bd",
				"iav_contactname": "Hernan Miranda",
				"new_openinghoursstartweekend": 808340020,
				"iav_openingdaysstartday": 808340014
			},
			"iav_unitid": {
				"name": "2733",
				"iav_smunitid": "PT1073",
				"iav_size": 1.5,
				"iav_unitcategorytext": "SS",
				"iav_subtype": "EL",
				"price": null,
				"iav_height": 3.0,
				"iav_width": 1.5,
				"iav_depth": 1.0,
				"_transactioncurrencyid_value": "444d83e5-a8f5-e511-93fe-00505691c4bd"
			},
			"iav_opportunityid": {
				"opportunityid": "2734e058-09ea-eb11-9455-00505691eca1",
				"iav_expectedmovein": "2021-06-30T22:00:00Z"
			},
			"iav_customerid": {
				"iav_documenttype": 808340000,
				"iav_dni": "47912703T",
				"emailaddress1": "jorge.castillo@quantion.com",
				"emailaddress2": null,
				"address1_telephone1": "660724077",
				"telephone1": null,
				"telephone2": null,
				"address1_telephone2": null,
				"address1_composite": "Rambla Catalana 58-60 A 3º 4ª\r\n08903 L' Hospitalet de Llobregat Barcelona\r\nSpain",
				"address1_line1": "Rambla Catalana 58-60 A 3º 4ª",
				"address1_line2": null,
				"address1_line3": null,
				"address1_city": "L' Hospitalet de Llobregat",
				"address1_postalcode": "08903",
				"address1_stateorprovince": "Barcelona",
				"address1_country": "Spain",
				"blue_updatewebportal": false,
				"blue_customertypestring": "Residential"
			}
		}
	]
}
 */