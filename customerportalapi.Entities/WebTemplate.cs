using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class WebTemplate
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("code")]
        public int Code { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonElement("data")]
        public string Data { get; set; }
    }
}
