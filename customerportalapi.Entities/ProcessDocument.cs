using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

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
