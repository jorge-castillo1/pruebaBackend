using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities.Enums
{
    public enum InviteInvocationType
    {
        CRM = 0, // default
        audit_trail_complete = 1,
        CronJob = 2        
    }
}
