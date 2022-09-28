using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IContractServices
    {
        Task<Contract> GetContractAsync(string contractNumber);

        Task<string> GetDownloadContractAsync(string dni, string contractNumber);

        Task<string> GetDownloadInvoiceAsync(InvoiceDownload invoiceDownload);

        Task<string> SaveContractAsync(Document document);

        Task<ContractFull> GetFullContractAsync(string contractNumber);

        Task<string> GetContractTimeZoneAsync(string contractNumber);

        Task<bool> DocumentExists(string smContractCode);

        Task<bool> InvoiceExists(string invoiceNumber);

        Task<UpdateContractsUrlResponse> UpdateContractUrlAsync(int? skip, int? limit);

        Task<SignatureResultDataResponse> UpdateContractsWithoutSignatureId(string fromCreatedOn,
            string toCreatedOn = null, string arrContracts = null, string status = null);

        Task<List<KeyValuePair<string, string>>> UploadDocuments(string arrContracts, string status = null);

        Task<ListContractStatusResponseList> UpdateContractStatusInCrm(List<ContractStatusRequest> contactListIds);
    }
}