using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class SiteInvoices
    {
        public string Name { get; set; }

        public string StoreCode { get; set; }

        public string StoreId { get; set; }

        public List<ContractInvoices> Contracts { get; set; } = new List<ContractInvoices>();
    }

    public class ContractInvoices
    {
        public string ContractId { get; set; }

        public string ContractNumber { get; set; }

        public string SmContractCode { get; set; }



        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
