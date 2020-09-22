using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class EkomiWidget
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ekomiCustomerId")]
        public string EkomiCustomerId { get; set; }

        [BsonElement("ekomiWidgetTokens")]
        public string EkomiWidgetTokens { get; set; }

        [BsonElement("ekomiLanguage")]
        public string EkomiLanguage { get; set; }

        [BsonElement("siteId")]
        public string SiteId { get; set; }

    }
}
