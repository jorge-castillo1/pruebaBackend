using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _services;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserServices services, ILogger<UsersController> logger)
        {
            _services = services;
            _logger = logger;
        }

        // GET api/users/{dni}
        [HttpGet("{dni}")]
        public async Task<ApiResponse> GetAsync(string dni)
        {
            try
            {
                _logger.LogInformation("Controller Users Dni!!!!!!!!!!!!!!");
                var entity = await _services.GetProfileAsync(dni);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpGet("sample/{dni}")]
        [AuthorizeToken]
        public async Task<ApiResponse> SampleGetAsync(string dni)
        {
            _logger.LogInformation("Accessing with authenticated users!!!!");

            var claimsPrincipal = HttpContext.User;
            _logger.LogInformation($"username {claimsPrincipal.Identity.Name}");           
            _logger.LogInformation($"user is in role customerportal_standard {claimsPrincipal.IsInRole("customerportal_standard")}");

            var claims = claimsPrincipal.FindFirst(x => x.Type == ClaimTypes.Email);
            _logger.LogInformation($"email logged {claims.Value}");


            return new ApiResponse();
        }

        // POST api/users
        [HttpPatch]
        public async Task<ApiResponse> PatchAsync([FromBody] Profile value)
        {
            try
            {
                var entity = await _services.UpdateProfileAsync(value);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        // POST api/users/invite
        [HttpPost("invite")]
        public async Task<ApiResponse> Invite([FromBody] Invitation value)
        {
            try
            {
                var entity = await _services.InviteUserAsync(value);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        // PUT api/users/confirm/{invitationToken}
        [HttpPut("confirm/{invitationToken}")]
        public async Task<ApiResponse> Confirm(string invitationToken)
        {
            try
            {
                var entity = await _services.ConfirmUserAsync(invitationToken);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        // PUT api/users/uninvite/{dni}
        [HttpPut("uninvite/{dni}")]
        public async Task<ApiResponse> UnInvite(string dni)
        {
            try
            {
                var entity = await _services.UnInviteUserAsync(dni);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        //// PUT api/users/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/users/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
