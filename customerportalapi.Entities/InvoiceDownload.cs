using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace customerportalapi.Entities
{
    public class InvoiceDownload
    {
        public string StoreCode { get; set; }

        public string InvoiceNumber { get; set; }

        public string Username { get; set; }
    }

}
