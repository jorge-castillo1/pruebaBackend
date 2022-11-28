using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Loggers;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
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
        private readonly IApiLogService _apiLogService;

        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserServices services, IApiLogService apiLogService, ILogger<UsersController> logger)
        {
            _services = services;
            _apiLogService = apiLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get User Profile
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User profile data model</returns>
        /// <remarks>
        /// This method checks if the user exists in the database.
        /// Check if the email is verified.
        /// Get the user's profile from the CRM API
        /// Check if the user's email is not repeated in the database. 
        /// If there is any error send email to **mailIT**
        /// Synchronize CRM profile data to the database
        /// </remarks>
        /// <response code = "200">User profile data model</response>
        /// <response code = "403">User is deactivated</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method checks if the user exists in the database.
        /// Checks if the email is verified.
        /// Obtains the crm profile by the dni and type of account.
        /// Synchronize changes, including language and avatar.
        /// </remarks>
        /// <response code = "200">User profile data model</response>
        /// <response code = "403">User is deactivated</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method first checks and sets the email.
        /// Updates all the user profile in the database and also in the CRM.
        /// Finally, it sends an email to the user informing that this has been updated
        /// </remarks>
        /// <response code = "200">Profile data updated</response>
        /// <response code = "400">Bad Request: 
        /// - Email field can not be null
        /// - Principal email can not be null
        /// </response>
        /// <response code = "403">User is deactivated</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// Invite users to customer portal and send welcome email
        /// </summary>
        /// <param name="value">Invitation data model</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// Use API KEY for this api.
        /// Validate that the email and DNI fields are informed
        /// Obtains the list of contracts associated with a DNI in CRM
        /// Verify that the user does not already exist(by Email), if it exists, send an email to "MailIT" with the template "ErrorInvitationEmailAlreadyExists"
        /// Check that the user exists in DB with that Email, Dni and CustomerType before continuing
        /// Obtains the user's profile in CRM
        /// Check and fill in the fields of all the systems involved: Contact(CRM), Contract(CRM, SM), Store(CRM), Unit(CRM), Opportunity(CRM)
        /// Verify that the mandatory fields are filled in, if it does not send an error mail to the "MailIT" mailbox, with the "InvitationError" template and returns an exception: "required some fields"
        /// Create the user or modify and establish a new temporary password
        /// Sends the WelcomeEmail(short or extended) depending on the configuration of the Features table and who invoked the process.From CRM always the Short, The rest according to configuration.
        /// Change the check of "WebPortalAccess" to true in CRM
        /// </remarks>
        /// <response code = "200">Boolean</response>
        /// <response code = "400">Controled Error:
        /// - User must have a valid email address.
        /// - User must have a valid document number.
        /// - Required some fields
        /// </response>
        /// <response code = "404"> Not found:
        /// - Email template not found
        /// - Store mail not found
        /// - Invitation user fails. Email in use by another user
        /// - Invitation user fails. User was activated before
        /// - Contact required: user: {userIdentification}
        /// - User without contract, user: {}userIdentification
        /// </response>
        /// <response code = "500">Internal Server Error</response>
        // POST api/users/invite
        [HttpPost("invite")]
        [CustomLog]
        public async Task<ApiResponse> Invite([FromBody] Invitation value)
        {
            try
            {
                var entity = await _services.InviteUserAsync(value);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                var obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("api/users/invite");
                var obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, $"POST .../api/users/invite. {ex.Message}. Invitation: {obj}");
                throw;
            }
        }

        /// <summary>
        /// Allow users to access customer portal after accepting invitation with new credentials
        /// </summary>
        /// <param name="receivedToken">Invitation token</param>
        /// <param name="value">New user credentials</param>
        /// <returns>Access Token</returns>
        /// <remarks>
        /// This method verifies that the user has a valid token
        /// Validates if the user is correct and unique
        /// Set Admincontact, Supercontact and WebPortalAcces to true and WebPortalUsername to the value of the user
        /// Updates the CRM profile.
        /// Add the user to the Identity Server
        /// Assign the groups to the user
        /// Updates the database
        /// Confirm acces to CRM
        /// And finally authorizes the user in identity and returns a token
        /// </remarks>
        /// <response code = "200">Access Token</response>
        /// <response code = "400">Bad Request: 
        /// - User must have a received Token
        /// - Wrong password
        /// - Username must not include @
        /// - Username must be unique
        /// </response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// First validates if the recievedToken is not empty
        /// Validates user by invitationToken or forgotPasswordToken
        /// Get UserProfile from CRM
        /// Set roles in CRM to true. Admincontact, Supercontact, WebPortalAccess
        /// Add user to the IdentityServer
        /// Assign groups/roles to the user
        /// Update email verification data
        /// Confirm acces status to external system
        /// </remarks>
        /// <response code = "200">Access Token</response>
        /// <response code = "400">User must have a receivedToken</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method searches the database and checks if the user exists.
        /// </remarks>
        /// <response code = "200">Boolean with result</response>        
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method searches the database and checks if the email exists.
        /// </remarks>
        /// <response code = "200">Boolean with result</response>
        /// <response code = "500">Internal Server Error</response>
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

        /// <summary>
        /// Uninvite user
        /// </summary>
        /// <param name="dni"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method first creates an invitation with dni and customer type residential
        /// Validates that the dni isn't empty
        /// Validates the username
        /// Deletes the user from Identity Server
        /// Deletes the user from the database
        /// Confirm revocation access status to external system & delete username in CRM
        /// </remarks>
        /// <response code = "200">Return True or False</response>
        /// <response code = "400">User must have a valid document number</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
        // PUT api/users/uninvite/{dni}
        [HttpPut("uninvite/{dni}")]
        [AuthorizeApiKey]
        [Obsolete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApiResponse> UnInviteDni(string dni)
        {
            try
            {
                var value = new Invitation()
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
        /// <remarks>
        /// Use API KEY for this api.
        /// Validates that the dni isn't empty.
        /// Validates the username.
        /// Deletes the user from Identity Server.
        /// Deletes the user from the database.
        /// Confirm revocation access status to external system & delete username in CRM.
        /// </remarks>        
        /// <response code = "200">Boolean with result</response>
        /// <response code = "400">User must have a valid document number</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
        // PUT api/users/uninvite/{dni}
        [HttpPut("uninvite")]
        [AuthorizeApiKey]
        [CustomLog]
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
        /// <remarks>
        /// Verifies that the username exists.
        /// Checks the type of the account
        /// Returns the info of the customer from the database
        /// </remarks>
        /// <response code = "200">Customer data model</response>
        /// <response code = "404">NotFound:
        /// - User does not exists
        /// - Account is not found
        /// </response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method searches the database via document number to find the information from the account
        /// </remarks>
        /// <response code = "200">Customer data model</response>
        /// <response code = "404">Account not found</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// This method is going to record by record updating with the new values introduced.
        /// Then updates the customer in the CRM
        /// </remarks>
        /// <response code = "200">Account data updated</response>
        /// <response code = "404">Account not found</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// </remarks>
        /// <response code = "200">Boolean with result</response>
        /// <response code = "400">FormContact Type field can not be null</response>
        /// <response code = "404">Not Found: 
        /// - Error retrieving the current user
        /// - User does not exist
        /// - User Profile does not exist
        /// </response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// Use API KEY for this api.
        /// First checks if the user and role are valid.
        /// Removes the user's current groups
        /// Finally adds user to the groups assigned
        /// </remarks>
        /// <response code = "200">True</response>
        /// <response code = "404">Not found:
        /// - User not found
        /// - Role not found
        /// </response>
        /// <response code = "500">Internal Server Error</response>
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
        /// Update user roles
        /// </summary>
        /// <param name="changeRoles">User Name and the list of roles</param>
        /// <returns></returns>
        /// <remarks>
        /// Use API KEY for this api.
        /// First validate the parameters
        /// Get the user from the database
        /// Get the user from Identity Server
        /// Remove all groups/roles from the user
        /// Validate name of the role
        /// Get role from IdentityServer
        /// Assign active roles to user
        /// </remarks>
        /// <response code = "200">True</response>
        /// <response code = "400">Bad Request:
        /// - User must have a valid email address.
        /// - User must have a valid document number.
        /// </response>
        /// <response code = "404">Not Found:
        /// - The role name cannot be empty
        /// - No roles have been sent
        /// </response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPatch("roles")]
        [AuthorizeApiKey]
        public async Task<ApiResponse> ChangeRoles([FromBody] ChangeRoles changeRoles)
        {
            try
            {
                var entity = await _services.ChangeRoles(changeRoles);
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
        /// Check if the user exists by email and dni 
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="dni">DNI</param>
        /// <param name="customerType">Customer Type, "Residential" or "Business"</param>
        /// <returns>True or False</returns>
        /// <remarks>
        /// This method first check the customertype of the user
        /// Search the database by dni and email to see if the record exists
        /// </remarks>
        /// <response code = "200">True or False</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("exist/{email}/{dni}/{customerType}")]
        public ApiResponse UserExistInDb(string email, string dni, string customerType = "Residential")
        {
            try
            {
                var entity = _services.UserExistInDb(email, dni, customerType).Result;

                if (string.IsNullOrEmpty(entity?.Id))
                    return new ApiResponse(false);

                return new ApiResponse(true);
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
        /// <remarks>Use API KEY for this api.
        /// Gets the user
        /// Gets the role
        /// Fins the group with that role
        /// Remove the user from the group
        /// </remarks>
        /// <response code = "200">Boolean with result</response>
        /// <response code = "404">User not found</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// <remarks>
        /// Validates that the token is not null
        /// The user is searched in the DB for the token
        /// Returns an object with the name of the user and their language</remarks>
        /// <response code = "200">Access Token</response>
        /// <response code = "400">User must have a recieved Token</response>
        /// <response code = "404">User not found</response>
        /// <response code = "500">Internal Server Error</response>
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

        /// <summary>
        /// Save new users inside the database
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// Insert the user in DB
        /// Mail is sent to the "MailWP" mailbox with the "SaveNewUser" template
        /// Returns true if it has been saved and false if not.
        /// </remarks>
        /// <response code = "200">True: User saved</response>
        /// <response code = "404">Store mail not found</response>
        /// <response code = "500">Internal Server Error</response>
        // POST api/users/newUser
        [HttpPost("newuser")]
        public async Task<ApiResponse> SaveNewUser([FromBody] NewUser newUser)
        {
            try
            {
                var entity = await _services.SaveNewUser(newUser);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {

                _logger.LogError(se, se.Message);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Save new users inside the database
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>- It makes a call to the Google Captcha service passing it a Token, 
        /// if the answer is 200 Ok it returns true, if not, it returns false
        /// </remarks>
        /// <response code = "200">True or False</response>
        /// <response code = "500">Internal Server Error</response>
        // POST api/users/validatecaptcha
        [HttpPost("validatecaptcha")]
        public async Task<ApiResponse> ValidateCaptcha([FromBody] string token)
        {
            try
            {
                var entity = await _services.ValidateCaptcha(token);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {

                _logger.LogError(se, se.Message);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
