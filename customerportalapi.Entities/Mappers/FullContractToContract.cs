using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace customerportalapi.Entities.Mappers
{
    public static class FullContractToContract
    {
        public static Contract Mapper(FullContract contract)
        {
            if (contract != null)
            {

                Contract newContract = new Contract()
                {
                    ContractId = contract.iav_contractid,
                    ContractNumber = contract.iav_name,
                    SmContractCode = contract.iav_smcontractcode,
                    Store = contract._iav_storeid_value,
                    Price = (decimal)contract.iav_price,
                    Vat = (decimal?)contract.iav_vat,
                    ReservationFee = contract.iav_reservationfee.ToString(),
                    ContractDate = contract.iav_contractdate.ToString(),
                    FirstPaymentDate = contract.iav_firstpaymentdate.ToString(),
                    FirstPayment = contract.iav_firstpaymentmoney.ToString(),
                    PaymentMethod = contract._iav_paymentmethod,
                    PaymentMethodId = contract._iav_paymentmethodid_value,
                    ContractUrl = contract.new_contacturl,
                    Customer = contract._iav_customerid_value,
                    Unit = contract.iav_unitid,
                    StoreData = contract.iav_storeid,
                    OpportunityId = contract._iav_opportunityid_value,

                    Currency = contract.iav_price_currency,
                    ExpectedMoveIn = contract.iav_opportunityid.iav_expectedmovein.ToString(),
                    MapLink = contract.iav_storeid.MapLink,
                    AccessType = contract.iav_storeid.AccessType,
                    PaymentMethodDescription = contract._iav_paymentmethod,
                    StoreCode = contract.iav_storeid.StoreCode,
                    StoreImage = contract.iav_storeid.StoreImage,
                    TimeZone = contract.iav_storeid.TimeZone,
                    ContractExpectedMoveIn = contract.blue_expectedmovein,
                    //TotalPrice = contract.iav_storeid,
                };

                var url = string.Empty;
                if (!string.IsNullOrEmpty(contract.iav_customerid?.blue_customertypestring) && !string.IsNullOrEmpty(contract.iav_customerid?.iav_dni) && !string.IsNullOrEmpty(contract.iav_storeid?.DocumentRepositoryUrl))
                {
                    url = $@"{contract.iav_storeid.DocumentRepositoryUrl}/Documentos compartidos/{contract.iav_customerid.blue_customertypestring}/{contract.iav_customerid.iav_dni}";
                }

                if (string.IsNullOrEmpty(contract.new_contacturl) || contract.new_contacturl != url)
                {
                    newContract.ContractUrl = url;
                }

                return newContract;
            }
            else
            {
                return new Contract() { };
            }
        }
    }
}
