using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface ILoginService
    {
        Task<Token> GetToken(Login credentials);

        Task<UserIdentity> ChangePassword(Login credentials);
    }
}
