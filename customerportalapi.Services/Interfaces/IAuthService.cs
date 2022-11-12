using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Token> RefreshToken(string token);
        Task<bool> Logout(string token);
    }
}