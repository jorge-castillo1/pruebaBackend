using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class StoreImage
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("storecode")]
        public string StoreCode { get; set; }

        [BsonElement("containerid")]
        public string ContainerId { get; set; }
    }
}