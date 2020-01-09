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
                    return "es";
                case "english":
                    return "en";
                case "portuguese":
                    return "pt";
                case "french":
                    return "fr";
                default:
                    return "en";
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

        public static string GetWelcomeMessage(string invitationLanguage)
        {
            switch (invitationLanguage.ToLower())
            {
                case "spanish":
                    return "Bienvenido a la área privada de clientes de Bluespace.";
                case "english":
                    return "Welcome Bluespace private customer portal.";
                case "portuguese":
                    return "Bem-vindo ao portal privado do cliente da Bluespace.";
                case "french":
                    return "Bienvenue sur le portail client privé de Bluespace.";
                default:
                    return "Welcome Bluespace private customer portal.";
            }
        }
    }
}
