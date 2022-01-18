using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class DocumentMetadata
    {
        public string DocumentId { get; set; }
        public int DocumentType { get; set; }
        public string StoreName { get; set; }
        public string AccountDni { get; set; }
        public int AccountType { get; set; }
        public string ContractNumber { get; set; }
        public string SmContractCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string RelativeUrl { get; set; }
        public string NewContractUrl { get; set; }
        public string BankAccountOrderNumber { get; set; }
        public string BankAccountName { get; set; }
        public string InvoiceNumber { get; set; }
        public string DomainUrl { get; set; }
    }
}
