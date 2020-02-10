using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace customerportalapi.Security
{
    public class AuthorizeTokenFilter : IAuthorizationFilter
    {
        readonly IConfiguration _config;
        readonly ILogger<AuthorizeTokenFilter> _logger;

        public AuthorizeTokenFilter(IConfiguration config, ILogger<AuthorizeTokenFilter> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var request = context.HttpContext.Request;

                // Get Authorization header value
                var authorization = request.Headers.FirstOrDefault(x => x.Key == HeaderNames.Authorization);
                if (authorization.Key == null || !authorization.Value[0].Contains("Bearer "))
                {
                    context.Result = new StatusCodeResult(499); //Token Required
                    return;
                }

                var token = authorization.Value[0].Split(' ');
                ClaimsPrincipal claims = JwtTokenHelper.GetPrincipal(token[1], _config);
                context.HttpContext.User = claims;
                Thread.CurrentPrincipal = context.HttpContext.User;

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
