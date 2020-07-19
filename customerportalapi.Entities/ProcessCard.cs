using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class ProcessCard
    {
        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("externalId")]
        public string ExternalId { get; set; }
    }
}
