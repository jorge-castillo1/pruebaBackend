using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Interfaces;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityRepository _identityRepository;

        public AuthService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public async Task<Token> RefreshToken(string token)
        {
            return await _identityRepository.RefreshToken(token);
        }

        public async Task<bool> Logout(string token)
        {
            return await _identityRepository.Logout(token);
        }
    }
}