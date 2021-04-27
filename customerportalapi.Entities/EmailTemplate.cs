using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

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

        [BsonElement("paragraphs")]
        public List<EmailParagraph> Paragraphs { get; set; }
    }

    public class EmailParagraph
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("defaultContent")]
        public string DefaultContent { get; set; }

        [BsonElement("customContent")]
        public string CustomContent { get; set; }
    }    
}
