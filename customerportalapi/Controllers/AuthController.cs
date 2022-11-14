using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
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

        // POST api/auth/refreshtoken
        /// <summary>
        /// Obtain a new Token from refresh token
        /// </summary>
        /// <param name="value">Refresh token</param>
        /// <returns>Token access</returns>
        /// <remarks>
        /// This method calls WSO2 Identity Server to refresh token and returns an updated token.
        /// </remarks>
        /// <response code = "200">Updated Token</response>
        /// <response code = "500">Handled error of type Internal Server Error</response>
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
        /// Logout from authenticator system
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// This method calls WSO2 Identity Server to identify the token from the user. Then logouts from the server.
        /// </remarks>
        /// <response code = "200">Logout Succesful</response>
        /// <response code = "500">Handled error of type Internal Server Error</response>
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