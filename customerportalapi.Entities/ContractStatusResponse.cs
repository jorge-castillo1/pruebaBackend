using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class ContractStatusResponse
    {
        // Signaturit
        public string SignatureId { get; set; }
        public string ExcelStatus { get; set; }
        public string StatusSignaturitNow { get; set; }

        // CRM
        public string CrmContractId { get; set; }
        public string CrmOldStatus { get; set; }
        public string CrmNewStatus { get; set; }
        public string CrmOldSignatureIdSignature { get; set; }
        public string CrmOldDocumentIdSignature { get; set; }
        public string CrmNewSignatureIdSignature { get; set; }
        public string CrmNewDocumentIdSignature { get; set; }
        public string CrmOldContractUrl { get; set; }
        public string CrmNewContractUrl { get; set; }
        public bool CrmUpdated { get; set; } = false;
    }

    public class ListContractStatusResponseList
    {
        public List<ContractStatusResponse> ListContractStatusResponse { get; set; } = new List<ContractStatusResponse>() { };
        public List<ListContractsNoProcessed> ListContractsNoProcessed { get; set; } = new List<ListContractsNoProcessed>() { };
        public List<SignatureResultData> ListSignatureResultData { get; set; } = new List<SignatureResultData>() { };
        public List<Entities.Contract> ListContracts { get; set; } = new List<Entities.Contract>() { };
    }

    public class ListContractsNoProcessed
    {
        public string SignatureId { get; set; }
        public string CrmContractId { get; set; }
    }
}
