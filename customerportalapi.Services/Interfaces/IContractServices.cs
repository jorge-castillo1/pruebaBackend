using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

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

    }
}
