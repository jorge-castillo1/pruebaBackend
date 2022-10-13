using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Feature
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("environments")]
        public List<FeatureEnvironment> Environments { get; set; }

        [BsonElement("countryavailable")]
        public List<string> CountryAvailable { get; set; }
    }

    public class FeatureEnvironment
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("value")]
        public object Value { get; set; }
    }
}
