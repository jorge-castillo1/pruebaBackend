using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace customerportalapi.Security
{
    public class AuthorizeTokenFilter : IAuthorizationFilter
    {
        readonly IIdentityRepository _identityRepository;
        readonly IConfiguration _config;
        readonly ILogger<AuthorizeTokenFilter> _logger;

        public AuthorizeTokenFilter(IIdentityRepository identityRepository, IConfiguration config, ILogger<AuthorizeTokenFilter> logger)
        {
            _identityRepository = identityRepository;
            _config = config;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var request = context.HttpContext.Request;

                // Get Authorization header value

                var useAzureMethodAuthentication = request.Headers.FirstOrDefault(x => x.Key == "use-azure-method-authentication");
                var authorization = request.Headers.FirstOrDefault(x => x.Key == HeaderNames.Authorization);
                if (authorization.Key == null || !authorization.Value[0].Contains("Bearer "))
                {
                    context.Result = new StatusCodeResult(499); //Token Required
                    return;
                }

                var h = new JwtSecurityTokenHandler();

                var token = authorization.Value[0].Split(' ');                
                //if (string.IsNullOrEmpty(useAzureMethodAuthentication.Value))
                //{
                    // Get claims from token
                    ClaimsPrincipal claims = JwtTokenHelper.GetPrincipal(token[1], _config);

                    // Validate against generator system
                    TokenStatus status = _identityRepository.Validate(token[1]).Result;
                    if (status.Active){

                    context.HttpContext.User = claims;
                    Thread.CurrentPrincipal = context.HttpContext.User;
                    }
                    else
                      throw new SecurityTokenExpiredException("Token expired");

                /*}
                else
                {

                    var tokenHandler = new JwtSecurityTokenHandler();
                    try
                    {
                        tokenHandler.ValidateToken(token[1], new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                        }, out SecurityToken validatedToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, String.Format("{0}:{1}", ex.Message, ex.StackTrace));
                        context.Result = new StatusCodeResult(500);   //Internal Server Error
                    }
                }*/

                return;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogError(ex, String.Format("{0}:{1}", ex.Message, ex.StackTrace));
                context.Result = new StatusCodeResult(498); //Invalid Token or expired
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, String.Format("{0}:{1}", ex.Message, ex.StackTrace));
                context.Result = new StatusCodeResult(500);   //Internal Server Error
            }
        }
    }
}
