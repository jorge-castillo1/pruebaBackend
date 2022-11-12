using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class Pay
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("externalId")]
        public string ExternalId { get; set; }

        [BsonElement("idcustomer")]
        public string Idcustomer { get; set; }

        [BsonElement("siteid")]
        public string Siteid { get; set; }

        [BsonElement("token")]
        public string Token { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("smContractCode")]
        public string SmContractCode { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("documentId")]
        public string DocumentId { get; set; }

        [BsonElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }

    }
}
