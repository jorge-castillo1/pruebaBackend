using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IProfileRepository
    {
        Task<Profile> GetProfileAsync(string dni);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<AccountCrm> GetAccountAsync(string dni);

        Task<AccountCrm> UpdateAccountAsync(AccountCrm account);
    }
}
