using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class ProcessPay
    {
        [BsonElement("externalId")]
        public string ExternalId { get; set; }
        [BsonElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }
    }
}
