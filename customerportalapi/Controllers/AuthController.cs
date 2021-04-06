using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

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

        /// <summary>
        /// Obtain a new Token from refresh token
        /// </summary>
        /// <param name="value">Refresh token</param>
        /// <returns>Token access</returns>
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
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Logout from autenticator system
        /// </summary>
        /// <returns>Boolean</returns>
        [HttpGet("logout")]
        public async Task<ApiResponse> LogoutAsync()
        {
            try
            {
                string authorization = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).ToString();
                var token = authorization.Split(' ')[1];
                var entity = await _service.Logout(token);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
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