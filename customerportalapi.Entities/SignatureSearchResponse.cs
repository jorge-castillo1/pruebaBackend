using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class SignatureSearchResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<SignatureProcess> Result { get; set; } = new List<SignatureProcess>();    
    }

    public class SignatureProcess
    {

        public SignatureResult SignatureResult { get; set; } = new SignatureResult();
        public string Store { get; set; }
        public List<ProcessedDocument> Documents { get; set; } = new List<ProcessedDocument>();
        public List<ProcessedUnit> Units { get; set; } = new List<ProcessedUnit>();
        public string UserIdentification { get; set; }
        public List<ProcessedRecipient> Recipients { get; set; } = new List<ProcessedRecipient>();
        public string SignatureChannel { get; set; }
        public string SignatureType { get; set; }
        public int AccountType { get; set; }
        public string AccountDni { get; set; }
    }

    public class ProcessedDocument
    {
        public int DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string BankAccountOrderNumber { get; set; }
        public string BankAccountName { get; set; }
        public string SmContractCode { get; set; }
    }

    public class ProcessedUnit
    {
        public string UnitIdentification { get; set; }
        public string UnitType { get; set; }
    }

    public class ProcessedRecipient
    {
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
    }
}
