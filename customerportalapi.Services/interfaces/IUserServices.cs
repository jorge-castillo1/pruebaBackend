using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string dni, string accountType);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<bool> InviteUserAsync(Invitation value);

        Task<Token> ConfirmUserAsync(string invitationToken);

        Task<bool> UnInviteUserAsync(Invitation value);

        Task<Account> GetAccountAsync(string dni, string accountType);

        Task<Account> UpdateAccountAsync(Account account);

        Task<bool> ContactAsync(FormContact value);

        Profile GetUserByUsername(string username);

        Task<bool> ChangeRole(string username, string role);

        Task<bool> RemoveRole(string username, string role);

    }
}
