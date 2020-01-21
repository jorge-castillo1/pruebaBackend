using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
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
