using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetContractsAsync(string dni, string accountType);

        Task<Contract> GetContractAsync(string smContractCode);

        Task<Contract> UpdateContractAsync(Contract cont);

        Task<List<FullContract>> GetFullContractsWithoutUrlAsync(int? limit);

        Task<List<FullContract>> GetFullContractsBySMCodeAsync(string code);
    }
}
