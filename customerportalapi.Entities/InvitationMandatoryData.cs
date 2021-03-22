using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class InvitationMandatoryData 
    {

        public bool Email { get; set; }
        public bool Dni { get; set; }
        public bool EmailInUse { get; set; }
        public bool Contact { get; set; }
        public bool Contract { get; set; }
        public bool ActiveContract { get; set; }
        public bool SmContractCode { get; set; }
        public bool Store { get; set; }
        public bool StoreId { get; set; }
        public bool OpeningDaysFirst { get; set; }
        public bool OpeningDaysLast { get; set; }
        public bool OpeningHoursFrom { get; set; }
        public bool OpeningHoursTo { get; set; }
        public bool StoreName { get; set; }
        public bool City { get; set; }
        public bool Opportunity { get; set; }
        public bool OpportunityId { get; set; }
        public bool ExpectedMoveIn { get; set; }

    }
}
