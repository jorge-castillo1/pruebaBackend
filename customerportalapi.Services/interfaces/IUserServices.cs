using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string username);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<bool> InviteUserAsync(Invitation value);

        Task<Token> ConfirmUserAsync(string invitationToken);

        Task<Token> ConfirmAndChangeCredentialsAsync(string invitationToken, ResetPassword value);

        Task<bool> UnInviteUserAsync(Invitation value);

        Task<Account> GetAccountAsync(string username);

        Task<Account> UpdateAccountAsync(Account account, string username);

        Task<bool> ContactAsync(FormContact value);

        Profile GetUserByUsername(string username);
        
        Task<bool> ChangeRole(string username, string role);

        Task<bool> RemoveRole(string username, string role);

        bool ValidateUsername(string username);
    }
}
