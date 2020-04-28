using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ProcessPay
    {
        [BsonElement("externalId")]
        public string ExternalId { get; set; }
        [BsonElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }
    }
}
