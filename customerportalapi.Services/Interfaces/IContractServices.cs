using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.interfaces
{
    public interface IContractServices
    {
        Task<Contract> GetContractAsync(string contractNumber);
        Task<string> GetDownloadContractAsync(string contractNumber);
        Task<ContractFull> GetFullContractAsync(string contractNumber);
        Task<string> GetContractTimeZoneAsync(string contractNumber);
    }
}
