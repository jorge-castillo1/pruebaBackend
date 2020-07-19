namespace customerportalapi.Entities
{
    public class PaymentMethodBank : PaymentMethod
    {
        public string ContractNumber { get; set; }
        public string SmContractCode { get; set; }
        public string IBAN { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string StoreCode { get; set; }
    }
}
