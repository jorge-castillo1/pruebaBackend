using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [BsonElement("dni")]
        public string dni { get; set; }
        [BsonElement("email")]
        public string email { get; set; }
        [BsonElement("language")]
        public string language { get; set; }
        [BsonElement("profilepicture")]
        public string profilepicture { get; set; }
        [BsonElement("usertype")]
        public int usertype { get; set; }
        [BsonElement("emailverified")]
        public bool emailverified { get; set; }
    }
}
