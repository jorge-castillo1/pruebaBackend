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
        public static string GenerateToken(IConfiguration config, string username, string role, string email, DateTime expirationDate)
        {
            var symmetricKey = Convert.FromBase64String(config["Identity:Credential:ClientSecret"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, username),
                            new Claim(ClaimTypes.Role, role),
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.Expiration, expirationDate.ToString())
                        }),

                Expires = expirationDate,

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

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

            var symmetricKey = Convert.FromBase64String(config["Identity:Credential:ClientSecret"]);
            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                ValidateLifetime = validateLifetime
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            return principal;
        }
    }
}
