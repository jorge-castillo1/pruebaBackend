namespace customerportalapi.Entities
{
    public class SignatureSearchFilterData
    {
        public string SignatureId { get; set; }
        public string Store { get; set; }
        public string DocumentType { get; set; }
        public string UnitIdentification { get; set; }
        public string UnitType { get; set; }
        public string UserIdentification { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string SignatureChannel { get; set; }
        public string SignatureType { get; set; }
        public string SignatureDateFrom { get; set; }
        public string SignatureDateTo { get; set; }
    }
}
