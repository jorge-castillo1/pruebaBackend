using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly ILogger<UsersController> _logger;

        public AuthController(IAuthService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST api/auth/refreshtoken
        [HttpPost("refreshtoken")]
        public async Task<ApiResponse> RefreshTokenAsync([FromBody] RefreshToken value)
        {
            try
            {
                var entity = await _service.RefreshToken(value.token);
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

        [HttpPost("logout")]
        public async Task<ApiResponse> LogoutAsync([FromBody] RefreshToken value)
        {
            try
            {
                var entity = await _service.Logout(value.token);
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
    }
}