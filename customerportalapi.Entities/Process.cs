﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Process
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("processtype")]
        public int ProcessType { get; set; }

        [BsonElement("processstatus")]
        public int ProcessStatus { get; set; }

        [BsonElement("documentid")]
        public string DocumentId { get; set; }

        [BsonElement("contractnumber")]
        public string ContractNumber { get; set; }

        [BsonElement("creationdate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("modifieddate")]
        public DateTime ModifiedDate { get; set; }
    }
}
