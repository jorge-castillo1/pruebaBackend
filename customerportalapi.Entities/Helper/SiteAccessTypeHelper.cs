using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities.Helper
{
    public static class SiteAccessTypeHelper
    {
        public const string Bearbox = "802260000";
        public const string Evalos = "802260001";
        public const string Storelogix = "802260002";

        public static string GetSiteAccessTypeOptionSet(string value)
        {
            switch (value)
            {
                case Bearbox:
                    return "Bearbox";
                case Evalos:
                    return "Evalos";
                case Storelogix:
                    return "Storelogix";
                default:
                    return null;
            }
        }
    }
}
