using customerportalapi.Entities;
using customerportalapi.Entities.enums;

namespace customerportalapi.Services
{
    public static class UserUtils
    {
        public static string GetLanguage(string invitationLanguage)
        {
            switch (invitationLanguage.ToLower())
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

        public static string GetAccountType(int userType)
        {
            switch(userType)
            {
                case (int)UserTypes.Residential:
                    return AccountType.Residential;
                case (int)UserTypes.Business:
                    return AccountType.Business;
                default:
                    return AccountType.Residential;
            }
        }
    }
}
