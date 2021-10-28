using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface ILoginService
    {
        Task<Token> GetToken(Login credentials);

        Task<Token> ChangePassword(ResetPassword credentials);

        Task<bool> SendNewCredentialsAsync(Login credentials);
    }
}
