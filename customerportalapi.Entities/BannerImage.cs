using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class BannerImage
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("imageurl")]
        public string ImageUrl { get; set; }

        [BsonElement("userlanguage")]
        public string UserLanguage { get; set; }

        [BsonElement("countrycode")]
        public string CountryCode { get; set; }

        [BsonElement("active")]
        public bool Active { get; set; }
    }
}
