using System;

namespace customerportalapi.Entities
{
    public class Contact
    {
        public string Fullname { get; set; }

        public string DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string EmailAddress1 { get; set; }

        public string EmailAddress2 { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string MobilePhone { get; set; }

        public string MobilePhone1 { get; set; }

        public string MobilePhone2 { get; set; }

        public string Address { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressCity { get; set; }

        public string AddressPostalCode { get; set; }

        public string AddressStateOrProvince { get; set; }

        public string AddressCountry { get; set; }

        public string Customer { get; set; }

        public int LanguageCode { get; set; }
    }
}
