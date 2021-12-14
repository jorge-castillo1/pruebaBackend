using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IProfileRepository
    {
        Task<Profile> GetProfileAsync(string dni, string accountType);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<AccountProfile> GetAccountAsync(string dni, string accountType);

        Task<AccountProfile> GetAccountByDocumentNumberAsync(string documentNumber);

        Task<AccountProfile> UpdateAccountAsync(AccountProfile account);

        Task<Profile> ConfirmedWebPortalAccessAsync(string dni, string accountType);

        Task<Profile> RevokedWebPortalAccessAsync(string dni, string accountType);

        Task<ProfilePermissions> GetProfilePermissionsAsync(string dni, string accountType);

        Task<AccountProfile> GetContactByMail(string mail);

    }
}
