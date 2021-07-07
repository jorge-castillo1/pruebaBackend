using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Account
    {
        public string SmCustomerId { get; set; }

        public string CompanyName { get; set; }

        public string DocumentNumber { get; set; }

        public string Phone1 { get; set; }

        public string Mobile1 { get; set; }

        public string Email1 { get; set; }

        public string Email2 { get; set; }

        public string UseThisAddress { get; set; }

        public List<Address> AddressList { get; set; }

        public string CustomerType { get; set; }

        public string Profilepicture { get; set; }

        public string PaymentMethodId { get; set; }

        public string Token { get; set; }

        public string BankAccount { get; set; }

        public string TokenUpdateDate { get; set; }
    }

    public class AccountProfile
    {
        public string SmCustomerId { get; set; }

        public string DocumentNumber { get; set; }

        public string CompanyName { get; set; }

        public string Phone1 { get; set; }

        public string MobilePhone1 { get; set; }

        public string EmailAddress1 { get; set; }

        public string EmailAddress2 { get; set; }

        public string Address1Street1 { get; set; }

        public string Address1Street2 { get; set; }

        public string Address1Street3 { get; set; }

        public string Address1City { get; set; }

        public string Address1StateOrProvince { get; set; }

        public string Address1PostalCode { get; set; }

        public string Address1Country { get; set; }

        public string Address2Street1 { get; set; }

        public string Address2Street2 { get; set; }

        public string Address2Street3 { get; set; }

        public string Address2City { get; set; }

        public string Address2StateOrProvince { get; set; }

        public string Address2PostalCode { get; set; }

        public string Address2Country { get; set; }

        public string AlternateStreet1 { get; set; }

        public string AlternateStreet2 { get; set; }

        public string AlternateStreet3 { get; set; }

        public string AlternateCity { get; set; }

        public string AlternateStateOrProvince { get; set; }

        public string AlternatePostalCode { get; set; }

        public string AlternateCountry { get; set; }

        public string UseThisAddress { get; set; }

        public string CustomerType { get; set; }

        public string Profilepicture { get; set; }
        
        public string PaymentMethodId { get; set; }

        public string BankAccount { get; set; }

        public string Token { get; set; }
        
        public string TokenUpdateDate { get; set; }

        public bool blue_updatewebportal { get; set; }
    }
}
