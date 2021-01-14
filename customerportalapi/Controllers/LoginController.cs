using System;
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
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service;
        private readonly ILogger<UsersController> _logger;

        public LoginController(ILoginService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate user against authenticator system
        /// </summary>
        /// <param name="value">Login credentials</param>
        /// <returns>Access Token if succesfully logged</returns>
        // POST api/login
        [HttpPost]
        public async Task<ApiResponse> PostAsync([FromBody] Login value)
        {
            try
            {
                var entity = await _service.GetToken(value);
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

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="value">Old and New password data</param>
        /// <returns>Access Token if succesfully changed password</returns>
        //POST api/login/passwordReset
        [HttpPost]
        [Route("passwordReset")]
        [AuthorizeToken]
        public async Task<ApiResponse> PasswordResetAsync([FromBody] ResetPassword value)
        {
            try
            {
                var entity = await _service.ChangePassword(value);
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

        /// <summary>
        /// Send mail to user with reset password instructions
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Boolean if mail send</returns>
        // POST api/login/forgotPassword
        [HttpPost("forgotPassword")]
        public async Task<ApiResponse> ForgotPassword([FromBody] Login credentials)
        {
            try
            {
                var entity = await _service.SendNewCredentialsAsync(credentials);
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
