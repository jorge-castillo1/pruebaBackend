using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.interfaces
{
    public interface IContractServices
    {
        Task<Contract> GetContractAsync(string contractNumber);
        Task<string> GetDownloadContractAsync(string dni, string contractNumber);
        Task<string> SaveContractAsync(Document document);
    }
}
