using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ProcessCard
    {
        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("externalId")]
        public string ExternalId { get; set; }

        [BsonElement("update")]
        public bool Update { get; set; }
    }
}
