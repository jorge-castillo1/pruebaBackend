using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantion.MongoDbLogger
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("loglevel")]
        public string LogLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("eventid")]
        public int EventId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("createdOn")]
        public DateTime CreatedOn { get; set; }
    }
}
