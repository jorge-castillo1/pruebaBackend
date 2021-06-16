using customerportalapi.Entities.enums;
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

namespace customerportalapi.Security
{
    public class AuthorizeAzureADFilter : IAuthorizationFilter
    {
        readonly IIdentityRepository _identityRepository;
        readonly IConfiguration _config;
        readonly ILogger<AuthorizeAzureADFilter> _logger;

        readonly List<RoleGroupTypes> _roleGroups;

        public AuthorizeAzureADFilter(IIdentityRepository identityRepository, IConfiguration config, ILogger<AuthorizeAzureADFilter> logger, RoleGroupTypes[] roleGroups)
        {
            _identityRepository = identityRepository;
            _config = config;
            _logger = logger;

            _roleGroups = new List<RoleGroupTypes>();
            _roleGroups.AddRange(roleGroups);
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
                // Get claims from token
                ClaimsPrincipal userClaims = JwtTokenAzuereADHelper.GetPrincipal(token[1], _config);

                // Validate against generator system
                JwtSecurityToken status;
                try
                {
                    status = _identityRepository.ValidateAzuereAD(token[1]);
                }
                catch
                {
                    throw new SecurityTokenExpiredException("Token expired");
                }

                if (status != null && status.ValidTo >= DateTime.UtcNow)
                {
                    context.HttpContext.User = userClaims;
                    Thread.CurrentPrincipal = context.HttpContext.User;
                }
                else
                    throw new SecurityTokenExpiredException("Token expired");

                // Validate groups                
                var userRoleGroups = GetRoleGroups(userClaims);
                if (!CheckAuthorizationForGroups(userRoleGroups))
                {
                    throw new System.UnauthorizedAccessException("Unauthorized");
                }

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

        private RoleGroupTypes[] GetRoleGroups(ClaimsPrincipal userClaims)
        {
            List<RoleGroupTypes> roleGroups = new List<RoleGroupTypes>();

            var claimsGroups = userClaims.Identities.FirstOrDefault().Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            if (claimsGroups != null && claimsGroups.Any())
            {
                foreach (var group in claimsGroups)
                {
                    if (!string.IsNullOrEmpty(group))
                    {                           
                        roleGroups.Add(EnumGuidMapperAttribute.GetEnum(Guid.Parse(group)));
                    }
                }
            }
            return roleGroups?.ToArray();
        }

        private bool CheckAuthorizationForGroups(RoleGroupTypes[] userRoleGroups)
        {
            // All endpoint groups are assigned to the user
            if (_roleGroups.All(p => userRoleGroups.Contains(p)))
            {
                return true;
            }
            return false;
        }
    }
}
