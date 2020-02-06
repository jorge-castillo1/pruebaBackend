using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class User
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("dni")]
        public string Dni { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonElement("profilepicture")]
        public string Profilepicture { get; set; }

        [BsonElement("usertype")]
        public int Usertype { get; set; }

        [BsonElement("emailverified")]
        public bool Emailverified { get; set; }

        [BsonElement("invitationtoken")]
        public string Invitationtoken { get; set; }

        [BsonElement("externalid")]
        public string ExternalId { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
    }
}
