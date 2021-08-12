using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace customerportalapi.Security
{
    public class AuthorizeApiKeyFilter : IAuthorizationFilter
    {
        readonly IConfiguration _config;
        readonly ILogger<AuthorizeApiKeyFilter> _logger;

        public AuthorizeApiKeyFilter(IConfiguration config, ILogger<AuthorizeApiKeyFilter> logger)
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
                if (authorization.Key == null)
                {
                    context.Result = new StatusCodeResult(499); //Token Required
                    return;
                }
                _logger.LogInformation("AUTHORIZATION VALUE");
                _logger.LogInformation(authorization.Value.ToString());
                _logger.LogInformation("AUTHORIZATION KEY");
                _logger.LogInformation(authorization.Key.ToString());
                _logger.LogInformation("API KEY");
                _logger.LogInformation(_config["CustomerPortal_ApiKey"]);

                if (authorization.Value != _config["CustomerPortal_ApiKey"])
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                    return;
                }
                
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, String.Format("{0}:{1}", ex.Message, ex.StackTrace));
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);   
            }
        }
    }
}
