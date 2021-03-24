using customerportalapi.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class InvitationMandatoryData 
    {

        public MandatoryData Email { get; set; }
        public MandatoryData Dni { get; set; }
        public MandatoryData NewEmail { get; set; }
        public MandatoryData FirstName { get; set; }
        public MandatoryData Contract { get; set; }
        public MandatoryData SmContractCode { get; set; }
        public MandatoryData ActiveContract { get; set; }
        public MandatoryData UnitPassword { get; set; }
        public MandatoryData UnitName { get; set; }
        public MandatoryData UnitLocation { get; set; }
        public MandatoryData Store { get; set; }
        public MandatoryData StoreId { get; set; }
        public MandatoryData OpeningDaysFirst { get; set; }
        public MandatoryData OpeningDaysLast { get; set; }
        public MandatoryData OpeningHoursFrom { get; set; }
        public MandatoryData OpeningHoursTo { get; set; }
        public MandatoryData StoreName { get; set; }
        public MandatoryData StoreEmail { get; set; }
        public MandatoryData StoreCity { get; set; }
        public MandatoryData Opportunity { get; set; }
        public MandatoryData OpportunityId { get; set; }
        public MandatoryData ExpectedMoveIn { get; set; }

    }

    public class MandatoryData
    {
        public string Value { get; set; }
        public StateEnum State { get; set; }
    }
}
