using customerportalapi.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IPayRepository
    {
        Pay Get(string username, string smContractCode);
        Pay GetByExternalId(string externalId);
        Pay Update(Pay pay);
        Task<bool> Create(Pay pay);
        Task<bool> Delete(Pay pay);
    }
}
