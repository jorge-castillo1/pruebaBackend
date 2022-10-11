using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace customerportalapi.Entities
{
    public class ApiLog
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("logged")]
        public DateTime Logged { get; set; }

        [BsonElement("traceId")]
        public string TraceId { get; set; }

        [BsonElement("actionid")]
        public string ActionId { get; set; }

        [BsonElement("level")]
        public string Level { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("methodname")]
        public string MethodName { get; set; }

        [BsonElement("httpmethod")]
        public string HttpMethod { get; set; }

        [BsonElement("path")]
        public string Path { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("controller")]
        public string Controller { get; set; }

        [BsonElement("action")]
        public string Action { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("remoteip")]
        public string RemoteIp { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("exceptionmessage")]
        public string ExceptionMessage { get; set; }

        [BsonElement("exceptiontrace")]
        public string ExceptionTrace { get; set; }
    }
}
