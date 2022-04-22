using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetCurrentUserByUsername(string username);
        User GetCurrentUserByEmail(string email);
        User GetCurrentUserByDniAndType(string dni, int userType);
        User GetCurrentUserByEmailAndDniAndType(string email, string dni, int userType);
        User Update(User user);
        User UpdateById(User user);
        Task<bool> Create(User user);
        Task<bool> Delete(User user);
        User GetUserByInvitationToken(string invitationToken);
        User GetUserByForgotPasswordToken(string forgotPasswordToken);
    }
}
