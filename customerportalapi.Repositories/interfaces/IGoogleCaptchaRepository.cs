using customerportalapi.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IGoogleCaptchaRepository
    {
        Task<bool> IsTokenValid(string responseToken);
    }
}
