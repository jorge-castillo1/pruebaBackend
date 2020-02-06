using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class User
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("dni")]
        public string dni { get; set; }

        [BsonElement("email")]
        public string email { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("password")]
        public string password { get; set; }

        [BsonElement("phone")]
        public string phone { get; set; }

        [BsonElement("language")]
        public string language { get; set; }

        [BsonElement("profilepicture")]
        public string profilepicture { get; set; }

        [BsonElement("usertype")]
        public int usertype { get; set; }

        [BsonElement("emailverified")]
        public bool emailverified { get; set; }

        [BsonElement("invitationtoken")]
        public string invitationtoken { get; set; }
    }
}
