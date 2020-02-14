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

        // POST api/login/forgotPassword
        [HttpPost("forgotPassword/{userName}")]
        public async Task<ApiResponse> ForgotPassword(string userName)
        {
            try
            {
                var entity = await _service.SendNewCredentialsAsync(userName);
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
        //[HttpPut("confirm/{invitationToken}")]
        //public async Task<ApiResponse> Confirm(string invitationToken)
        //{
        //    try
        //    {
        //        var entity = await _services.ConfirmUserAsync(invitationToken);
        //        return new ApiResponse(entity);
        //    }
        //    catch (ServiceException se)
        //    {
        //        return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        throw;
        //    }
        //}
    }
}
