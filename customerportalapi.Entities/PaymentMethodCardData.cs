namespace customerportalapi.Entities
{
    public class PaymentMethodCardData
    {
        public string ExternalId { get; set; }
        public string idCustomer { get; set; }
        public string SiteId { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string CardHolder { get; set; }
        public string expiryDate { get; set; }
        public string typeCard { get; set; }
        public string cardNumber { get; set; }

    }
}
