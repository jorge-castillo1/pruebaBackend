using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class StoreImageUrl
    {
        public string StoreCode { get; set; }

        public string DocumentUrl { get; set; }
    }
}