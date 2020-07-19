namespace customerportalapi.Entities
{
    public class SMBankAccount
    {
        public string CustomerId { get; set; }
        public string PaymentMethodId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public int Default { get; set; }
        public string Iban { get; set; }

    }
}
