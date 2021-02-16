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

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
    }
}
