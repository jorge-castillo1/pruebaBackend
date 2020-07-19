namespace customerportalapi.Entities
{
    public class SignatureStatus
    {
        public string SignatureId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentManagementId { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        //public SignatureStatusMetadata Metadata { get; set; }
    }
}
