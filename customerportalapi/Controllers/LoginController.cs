using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
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
        /// <remarks>This method first searches the database by username if not by email
        /// If the login exists, it authorizes in the Identity Server and returns a token, updating the login attempts
        /// </remarks>
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
        /// <remarks>
        /// This method gets the user from the database, first checks if the old password is valid. 
        /// If it is correct, it updates the user with the new password in the Identity Server.
        /// Finally, it passes to null the password in our database.
        /// </remarks>
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
        /// Send mail to user with reset password instructions
        /// </summary>
        /// <param name="credentials">Login data</param>
        /// <returns>Boolean if mail send</returns>
        /// <remarks>
        /// This method generates a random password and saves it in the database.
        /// Email is sent in the language selected by the user with the password reminder.
        /// </remarks>
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
                _logger.LogError(se.ToString());
                if (se.Field == FieldNames.UserOrEmail && se.FieldMessage == ValidationMessages.NotExist || se.Field == FieldNames.User && se.FieldMessage == ValidationMessages.InvitationNotAccepted)
                {
                    return new ApiResponse(true);
                }
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
