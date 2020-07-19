namespace customerportalapi.Entities
{
    public class PaymentMethodGetCardResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public string expirydate { get; set; }
        public string cardnumber { get; set; }
        public string card_holder { get; set; }
    }
}
