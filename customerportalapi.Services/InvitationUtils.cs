using customerportalapi.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Services
{
    public static class InvitationUtils
    {
        public static string GetLanguage(string invitationLanguage)
        {
            switch(invitationLanguage.ToLower())
            {
                case "spanish":
                    return LanguageTypes.es.ToString();
                case "english":
                    return LanguageTypes.en.ToString();
                case "portuguese":
                    return LanguageTypes.pt.ToString();
                case "french":
                    return LanguageTypes.fr.ToString();
                default:
                    return LanguageTypes.en.ToString();
            }
        }

        public static int GetUserType(string invitationCustomerType)
        {
            switch (invitationCustomerType.ToLower())
            {
                case "residential":
                    return (int)UserTypes.Residential;
                case "business":
                    return (int)UserTypes.Business;
                default:
                    return (int)UserTypes.Residential; 
            }
        }
    }
}
