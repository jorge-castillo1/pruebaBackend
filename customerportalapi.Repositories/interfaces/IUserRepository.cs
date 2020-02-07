using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IUserRepository
    {
        User GetCurrentUser(string username);
        User GetCurrentUserByDni(string dni);
        User Update(User user);
        Task<bool> Create(User user);
        Task<bool> Delete(User user);
        User GetUserByInvitationToken(string invitationToken);
    }
}
