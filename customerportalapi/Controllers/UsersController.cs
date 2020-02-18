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
        [HttpGet("{dni}/Residential")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetAsync(string dni)
        {
            try
            {
                var entity = await _services.GetProfileAsync(dni, AccountType.Residential);
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

        [HttpGet("{dni}/Business")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetBusinessAsync(string dni)
        {
            try
            {
                var entity = await _services.GetProfileAsync(dni, AccountType.Business);
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
        [AuthorizeApiKey]
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
        [AuthorizeApiKey]
        [Obsolete]
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
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

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
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        // GET api/users/accounts/{dni}
        [HttpGet("accounts/{dni}")]
        [HttpGet("accounts/{dni}/Residential")]
        public async Task<ApiResponse> GetAccountAsync(string dni)
        {
            try
            {
                var entity = await _services.GetAccountAsync(dni, AccountType.Residential);
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

        [HttpGet("accounts/{dni}/Business")]
        public async Task<ApiResponse> GetAccountBusinessAsync(string dni)
        {
            try
            {
                var entity = await _services.GetAccountAsync(dni, AccountType.Business);
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

        // POST api/users/accounts
        [HttpPatch("accounts")]
        public async Task<ApiResponse> PatchAccountAsync([FromBody] Account value)
        {
            try
            {
                var entity = await _services.UpdateAccountAsync(value);
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
