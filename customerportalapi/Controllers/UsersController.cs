using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User profile data model</returns>
        // GET api/users/{dni}
        [HttpGet("{username}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetAsync(string username)
        {
            try
            {
                var entity = await _services.GetProfileAsync(username);
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
        /// Get User Profile
        /// </summary>
        /// <param name="dni">User Document Identification Number</param>
        /// <param name="accountType">Account type</param>
        /// <returns>User profile data model</returns>
        // GET api/users/{dni}
        [HttpGet("{dni}/{accountType}")]
        //[AuthorizeAzureAD(new[] { Entities.enums.RoleGroupTypes.StoreManager })]
        public async Task<ApiResponse> GetUserByDniAndTypeAsync(string dni, string accountType)
        {
            try
            {
                var entity = await _services.GetProfileByDniAndTypeAsync(dni, accountType);
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
        /// Update users profile
        /// </summary>
        /// <param name="value">Profile data to update</param>
        /// <returns>Profile data updated</returns>
        // POST api/users
        [HttpPatch]
        [AuthorizeToken]
        public async Task<ApiResponse> PatchAsync([FromBody] Profile value)
        {
            try
            {
                var entity = await _services.UpdateProfileAsync(value);
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
        /// Invite users to customer portal
        /// </summary>
        /// <param name="value">Invitation data model</param>
        /// <returns>Boolean</returns>
        /// <remarks>Use API KEY for this api</remarks>
        // POST api/users/invite
        [HttpPost("invite")]
        //[AuthorizeApiKey]
        public async Task<ApiResponse> Invite([FromBody] Invitation value)
        {
            try
            {
                var entity = await _services.InviteUserAsync(value);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Allow users to access customer portal after accepting invitation with new credentials
        /// </summary>
        /// <param name="receivedToken">Invitation token</param>
        /// <param name="value">New user credentials</param>
        /// <returns>Access Token</returns>
        [HttpPut("confirm/user/{receivedToken}")]
        public async Task<ApiResponse> ConfirmAndChangeCredentials(string receivedToken, [FromBody] ResetPassword value)
        {
            try
            {
                var entity = await _services.ConfirmAndChangeCredentialsAsync(receivedToken, value);
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
        /// Allow users to access customer portal after accepting invitation
        /// </summary>
        /// <param name="receivedToken">Invitation token</param>
        /// <returns>Access Token</returns>
        // PUT api/users/confirm/{receivedToken}
        [HttpPut("confirm/{receivedToken}")]
        public async Task<ApiResponse> Confirm(string receivedToken)
        {
            try
            {
                var entity = await _services.ConfirmUserAsync(receivedToken);
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
        /// validate user from Access Token
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Boolean with result</returns>
        [HttpGet("{username}/validation")]
        public ApiResponse UniqueUsername(string username)
        {
            try
            {
                bool entity = _services.ValidateUsername(username);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

       
        /// <summary>
        /// validate user from Access Token
        /// </summary>
        /// <param name="email">Username</param>
        /// <returns>Boolean with result</returns>
        [HttpGet("{email}/validate")]
        public ApiResponse EmailExists(string email)
        {
            try
            { 
                bool entity = _services.ValidateEmail(email);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        // PUT api/users/uninvite/{dni}
        [HttpPut("uninvite/{dni}")]
        [AuthorizeApiKey]
        [Obsolete]
        [ApiExplorerSettings(IgnoreApi =true)]
        public async Task<ApiResponse> UnInviteDni(string dni)
        {
            try
            {
                Invitation value = new Invitation()
                {
                    Dni = dni,
                    CustomerType = AccountType.Residential
                };
                var entity = await _services.UnInviteUserAsync(value);
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
        /// Revoke customer portal access to users
        /// </summary>
        /// <param name="value">User invitation data to revoke</param>
        /// <returns>Boolean with result</returns>
        /// <remarks>Use API KEY for this api</remarks>
        // PUT api/users/uninvite/{dni}
        [HttpPut("uninvite")]
        [AuthorizeApiKey]
        public async Task<ApiResponse> UnInvite([FromBody] Invitation value)
        {
            try
            {
                var entity = await _services.UnInviteUserAsync(value);
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
        /// Get customer information from username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Customer data model</returns>
        // GET api/users/accounts/{username}
        [HttpGet("accounts/{username}")]
        public async Task<ApiResponse> GetAccountAsync(string username)
        {
            try
            {
                var entity = await _services.GetAccountAsync(username);
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
        /// Get customer information from document number
        /// </summary>
        /// <param name="documentNumber">Document Number</param>
        /// <returns>Customer data model</returns>
        // GET api/users/accounts/{documentNumber}/base
        [HttpGet("accounts/{documentNumber}/base")]
        public async Task<ApiResponse> GetAccountBydocumentNumberAsync(string documentNumber)
        {
            try
            {
                var entity = await _services.GetAccountByDocumentNumberAsync(documentNumber);
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
        /// Update customer information
        /// </summary>
        /// <param name="value">Account data model to update</param>
        /// <param name="username">Username</param>
        /// <returns>Account data updated</returns>
        // POST api/users/accounts
        [HttpPatch("accounts/{username}")]
        public async Task<ApiResponse> PatchAccountAsync([FromBody] Account value, string username)
        {
            try
            {
                var entity = await _services.UpdateAccountAsync(value, username);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Associate contact information to user 
        /// </summary>
        /// <param name="value">Contact Information</param>
        /// <returns>Boolean with result</returns>
        // POST api/users/contact
        [HttpPost("contact")]
        [AuthorizeToken]
        public async Task<ApiResponse> Contact([FromBody] FormContact value)
        {
            try
            {
                var entity = await _services.ContactAsync(value);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Update user roles
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="role">role</param>
        /// <returns></returns>
        /// <remarks>Use API KEY for this api</remarks>
        [HttpPatch("role/{username}/{role}")]
        [AuthorizeApiKey]
        public async Task<ApiResponse> ChangeRole(string username, string role)
        {
            try
            {
                var entity = await _services.ChangeRole(username, role);
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
        /// Remove user role
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="role">Role to remove</param>
        /// <returns>Boolean with result</returns>  
        /// <remarks>Use API KEY for this api</remarks>
        [HttpPatch("role/remove/{username}/{role}")]
        [AuthorizeApiKey]
        public async Task<ApiResponse> RemoveRole(string username, string role)
        {
            try
            {
                var entity = await _services.RemoveRole(username, role);
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
        /// Get Profile (only some fields) using InvitationToken
        /// </summary>
        /// <param name="receivedToken">Invitation token</param>
        /// <returns>Access Token</returns>
        [HttpGet("token/{receivedToken}")]
        public async Task<ApiResponse> GetUserByInvitationtokenAsync(string receivedToken)
        {
            try
            {
                var entity = await _services.GetUserByInvitationTokenAsync(receivedToken);
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
