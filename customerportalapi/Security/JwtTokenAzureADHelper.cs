using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace customerportalapi.Security
{
    public static class JwtTokenAzuereADHelper
    {
        public static ClaimsPrincipal GetPrincipal(string token, IConfiguration config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            bool validateLifetime = true;
#if DEBUG
            validateLifetime = false;
#endif

            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity azureADIdentity = new ClaimsIdentity("AzuereADIdentity");

            //Claim de identidad
            Claim identidad = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub");
            var id = identidad.Value;

            if (identidad != null)
                if (identidad.Value.IndexOf("@") != -1)
                {
                    id = identidad.Value.Substring(0, identidad.Value.IndexOf("@"));
                }

            azureADIdentity.AddClaim(new Claim(ClaimTypes.Name, id));

            //Roles
            foreach (var claim in jwtToken.Claims.Where(x => x.Type == "groups"))
                azureADIdentity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));

            //Email Claim
            Claim email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");
            if (email != null)
                azureADIdentity.AddClaim(new Claim(ClaimTypes.Email, email.Value));

            //Expiration Claim
            Claim expiracion = jwtToken.Claims.FirstOrDefault(x => x.Type == "exp");
            if (expiracion != null)
                azureADIdentity.AddClaim(new Claim(ClaimTypes.Expiration, expiracion.Value));


            principal.AddIdentity(azureADIdentity);
            return principal;
        }
    }
}
