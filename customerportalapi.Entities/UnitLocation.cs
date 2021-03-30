using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class UnitLocation
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("sitecode")]
        public string SiteCode { get; set; }
        [BsonElement("sizecode")]
        public string SizeCode { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
    }
}
