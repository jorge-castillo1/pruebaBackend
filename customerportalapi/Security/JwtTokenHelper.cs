using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace customerportalapi.Security
{
    public static class JwtTokenHelper
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

            /*             var symmetricKey = Convert.FromBase64String(config["Identity:Credential:ClientSecret"]);
                        var validationParameters = new TokenValidationParameters()
                        {
                            RequireExpirationTime = true,
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                            ValidateLifetime = validateLifetime,
                            ValidateIssuerSigningKey = false
                        };

                       SecurityToken securityToken;
                       var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken); */
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity("IdentityServer");


            //Claim de identidad
            Claim identidad = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub");
            if (identidad != null)
                identity.AddClaim(new Claim(ClaimTypes.Name, identidad.Value));

            //Roles
            foreach (var claim in jwtToken.Claims.Where(x => x.Type == "groups"))
                identity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));

            //Email Claim
            Claim email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");
            if (email != null)
                identity.AddClaim(new Claim(ClaimTypes.Email, email.Value));

            //Expiration Claim
            Claim expiracion = jwtToken.Claims.FirstOrDefault(x => x.Type == "exp");
            if (expiracion != null)
                identity.AddClaim(new Claim(ClaimTypes.Expiration, expiracion.Value));

            //PreferedName Claim
            Claim prefereredName = jwtToken.Claims.FirstOrDefault(x => x.Type == "preferred_username");
            if (prefereredName != null)
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, prefereredName.Value));

            principal.AddIdentity(identity);
            return principal;
        }
    }
}
