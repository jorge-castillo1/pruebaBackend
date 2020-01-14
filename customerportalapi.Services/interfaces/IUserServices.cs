using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string dni);

        Task<Profile> UpdateProfileAsync(Profile profile);

        Task<bool> InviteUserAsync(Invitation value);

        Task<bool> ConfirmUserAsync(string invitationToken);
    }
}
