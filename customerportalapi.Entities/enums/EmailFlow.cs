using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities.enums
{
    public enum EmailFlow
    {      
        DownloadContract,
        DownloadInvoice,
        SendNewCredentials,
        UpdatePayment,
        UpdatePaymentCard,
        UpdateAccessCode,
        GetProfile,
        UpdateProfile,
        InviteUser,
        SendWelcome,
        UpdateAccount,
        ContactCall,
        ContactContact,
        Contact,
        SendMailInvitationError,
        SaveNewUser,
        ExceptionDocumetStoreApi
    }
}
