using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string username);

        Task<Profile> GetProfileByDniAndTypeAsync(string dni, string accountType);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<bool> InviteUserAsync(Invitation value);

        Task<InvitationMandatoryData> FindInvitationMandatoryData(Invitation invitationValues, Profile profile);

        Task<Token> ConfirmUserAsync(string invitationToken);

        Task<Token> ConfirmAndChangeCredentialsAsync(string invitationToken, ResetPassword value);

        Task<bool> UnInviteUserAsync(Invitation value);

        Task<Account> GetAccountAsync(string username);

        Task<Account> GetAccountByDocumentNumberAsync(string documentNumber);

        Task<Account> UpdateAccountAsync(Account account, string username);

        Task<bool> ContactAsync(FormContact value);

        Profile GetUserByUsername(string username);

        Task<bool> ChangeRole(string username, string role);

        Task<User> UserExistInDb(string email, string dni, string customerType);

        Task<bool> ChangeRoles(ChangeRoles changeRoles);

        Task<bool> RemoveRole(string username, string role);

        bool ValidateUsername(string username);

        bool ValidateEmail(string email);

        Task<Profile> GetUserByInvitationTokenAsync(string receivedToken);

        Task<bool> SaveNewUser(NewUser newUser);

        Task<bool> ValidateCaptcha(string token);

        Task<bool> TrimUserData();
    }
}
