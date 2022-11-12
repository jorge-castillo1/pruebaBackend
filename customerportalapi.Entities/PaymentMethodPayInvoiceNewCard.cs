using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodPayInvoiceNewCard
    {
        public bool Recurrent { get; set; }
        public string ExternalId { get; set; }
        public string Channel { get; set; }
        public string SiteId { get; set; }
        public string IdCustomer { get; set; }
        public string Nif { get; set; }
        public string Name { get; set; }
        public string Surnames { get; set; }
        public string Url { get; set; }
        public decimal Amount { get; set; }
        public string Ourref { get; set; }
        public string DocumentId { get; set; }
        public string Username { get; set; }
        public string SmContractCode { get; set; }
        public string Language { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhonePrefix { get; set; }
        public string CountryISOCodeNumeric { get; set; }
        public Address Address { get; set; }
    }
}
