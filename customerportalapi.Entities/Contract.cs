namespace customerportalapi.Entities
{
    public class Contract
    {
        public string ContractNumber { get; set; }

        public string Store { get; set; }

        public string Currency { get; set; }

        public string Price { get; set; }

        public string Vat { get; set; }

        public string ReservationFee { get; set; }

        public string ContractDate { get; set; }

        public string FirstPaymentDate { get; set; }

        public string FirstPayment { get; set; }

        public string PaymentMethod { get; set; }

        public string Customer { get; set; }

        public Unit Unit { get; set; }

        public Store StoreData { get; set; }
    }
}
