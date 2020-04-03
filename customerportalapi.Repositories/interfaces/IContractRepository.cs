using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetContractsAsync(string dni, string accountType);
        Task<Contract> GetContractAsync(string contractNumber);
        Task<string> GetDownloadContractAsync(string contractNumber);
    }
}
