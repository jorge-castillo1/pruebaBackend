using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class EmailTemplate
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement]
        public int code { get; set; }

        [BsonElement]
        public string subject { get; set; }

        [BsonElement]
        public string body { get; set; }

        [BsonElement]
        public string language { get; set; }
    }
}
