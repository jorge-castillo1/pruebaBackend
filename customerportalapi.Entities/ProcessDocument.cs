using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class ProcessDocument
    {
        [BsonElement("documentid")]
        public string DocumentId { get; set; }

        [BsonElement("documentstatus")]
        public string DocumentStatus { get; set; }
    }
}
