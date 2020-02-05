using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace customerportalapi.Security
{
    public class SchemeHandler : IAuthenticationHandler
    {
        private HttpContext _context;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            // do something
            return null;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            properties = properties ?? new AuthenticationProperties();
            _context.Response.StatusCode = 403;

            return Task.CompletedTask;
        }
    }
}
