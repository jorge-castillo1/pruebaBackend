using Newtonsoft.Json;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class SiteInvoices
    {
        public string ContractNumber { get; set; }

        public string Name { get; set; }

        public string StoreCode { get; set; }

        public string StoreId { get; set; }

        public string CustomerId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ContractInvoices> Contracts { get; set; } = new List<ContractInvoices>();

        public List<Invoice> Documents { get; set; } = new List<Invoice>();
    }

    public class ContractInvoices
    {
        public string ContractId { get; set; }

        public string ContractNumber { get; set; }

        public string SmContractCode { get; set; }

        public string StoreCode { get; set; }

        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
