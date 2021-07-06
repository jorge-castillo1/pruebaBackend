using customerportalapi.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class InvitationMandatoryData 
    {
        public MandatoryData ContactUsername { get; set; }
        public MandatoryData Contract { get; set; }
        public MandatoryData SmContractCode { get; set; }
        public MandatoryData SMContract { get; set; }
        public MandatoryData ActiveContract { get; set; }

        //Access Code eliminado temporalmente de Mandatory Data
        //public MandatoryData UnitPassword { get; set; }
        public MandatoryData UnitName { get; set; }
        public MandatoryData ContractStoreCode { get; set; }
        public MandatoryData UnitSizeCode { get; set; }
        public MandatoryData StoreCode { get; set; }
        public MandatoryData OpeningDaysFirst { get; set; }
        public MandatoryData OpeningDaysLast { get; set; }
        public MandatoryData OpeningHoursFrom { get; set; }
        public MandatoryData OpeningHoursTo { get; set; }
        public MandatoryData StoreName { get; set; }
        public MandatoryData StoreEmail { get; set; }
        public MandatoryData StoreCity { get; set; }
        public MandatoryData ContractOpportunity { get; set; }
        public MandatoryData OpportunityId { get; set; }
        public MandatoryData ExpectedMoveIn { get; set; }

    }

    public class MandatoryData
    {
        public string Value { get; set; }
        public StateEnum State { get; set; }
        public SystemTypes System { get; set; }
        public EntityNames Entity { get; set; }

        public void SetValueAndState(string value, StateEnum state)
        {
            this.Value = value;
            this.State = state;
        }

    }
}
