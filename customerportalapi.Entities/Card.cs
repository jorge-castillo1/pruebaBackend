using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class Card
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
        public int Status { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("cardholder")]
        public string Cardholder { get; set; }

        [BsonElement("expirydate")]
        public string Expirydate { get; set; }

        [BsonElement("typecard")]
        public string Typecard { get; set; }

        [BsonElement("cardnumber")]
        public string Cardnumber { get; set; }

        [BsonElement("contractNumber")]
        public string ContractNumber { get; set; }

        [BsonElement("smContractCode")]
        public string SmContractCode { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("current")]
        public bool Current { get; set; }

        [BsonElement("documentId")]
        public string DocumentId { get; set; }

    }
}
