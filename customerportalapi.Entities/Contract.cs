namespace customerportalapi.Entities
{
    public class Contract
    {
        public string ContractId { get; set; }

        public string ContractNumber { get; set; }

        public string SmContractCode { get; set; }

        public string Store { get; set; }

        public string StoreCode { get; set; }

        public string Currency { get; set; }

        public decimal Price { get; set; }

        public decimal? Vat { get; set; }

        public decimal TotalPrice { get; set; }

        public string ReservationFee { get; set; }

        public string ContractDate { get; set; }

        public string FirstPaymentDate { get; set; }

        public string FirstPayment { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentMethodId { get; set; }

        public string PaymentMethodDescription { get; set; }

        public string Customer { get; set; }

        public Unit Unit { get; set; }

        public Store StoreData { get; set; }

        public string AccessType {get; set; }

        public string TimeZone { get; set; }

        public string MapLink { get; set; }
        
        public string OpportunityId { get; set; }

        public string ExpectedMoveIn { get; set; }

        public string StoreImage { get; set; }

    }
}
