using AutoWrapper.Wrappers;
using customerportalapi.Entities;
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
    public class SitesController : ControllerBase
    {
        private readonly ISiteServices _services;
        private readonly IContractServices _contractService;
        private readonly ILogger<SitesController> _logger;


        public SitesController(ISiteServices services, IContractServices contractService, ILogger<SitesController> logger)
        {
            _services = services;
            _contractService = contractService;
            _logger = logger;
        }

        /// <summary>
        /// Get sites list where current user has active contracts
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Site list</returns>
        /// <remarks>
        /// - Get the user from DB by `username` and it is verified that it exists.
        /// - Get a list of contracts by DNI
        /// - Get Banner from DB
        /// - For each contract, it obtains its information in SM, and returns it.
        /// </remarks>
        /// <response code = "200">List of sites</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("users/{username}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetAsync(string username)
        {
            try
            {
                var entity = await _services.GetContractsAsync(username);
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
        /// Get sites list where current user has active contracts
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Site list</returns>
        /// <remarks>
        /// - Get the user from DB by `username` and it is verified that it exists.
        /// - Get a list of contracts by DNI
        /// - Get Banner from DB
        /// - For each contract, it obtains its information in SM, and returns it.
        /// </remarks>
        /// <response code = "200">List of sites</response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("users/{username}/msadal")]
        //[AuthorizeAzureAD(new[] { Entities.enums.RoleGroupTypes.StoreManager })]
        public async Task<ApiResponse> GetContractsAsync(string username)
        {
            try
            {
                var entity = await _services.GetContractsAsync(username);
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
        /// Search stores by country and/or city
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <param name="city">City</param>
        /// <param name="skip">page number</param>
        /// <param name="limit">page size</param>
        /// <returns>List of stores</returns>
        /// <remarks>
        /// - Get a list of stores in CRM by country code or city
        /// </remarks>
        /// <response code = "200">List of stores</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("stores")]
        //[AuthorizeToken]
        public async Task<ApiResponse> GetAsync(string countryCode, string city, int skip, int? limit)
        {
            try
            {
                if (limit == null)
                {
                    var entity = await _services.GetStoresAsync(countryCode, city);
                    return new ApiResponse(entity);
                }
                else
                {
                    var entity = await _services.GetPaginatedStoresAsync(countryCode, city, skip, (int)limit);
                    return new ApiResponse(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get unit information from unique unit id
        /// </summary>
        /// <param name="id">Unit identification Id</param>
        /// <returns>Unit data model</returns>
        /// <remarks>
        /// Get unit information in CRM by id
        /// </remarks>
        /// <response code = "200">Unit information</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("units/{id:guid}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetUnitAsync(Guid id)
        {
            try
            {
                var entity = await _services.GetUnitAsync(id);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get unit information from unique erp system unit id
        /// </summary>
        /// <param name="smid">ERP Unit identification Id </param>
        /// <returns>Unit data model</returns>
        /// <remarks>
        /// Get unit information in CRM by SM id
        /// </remarks>
        /// <response code = "200">Unit information</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("units/{smid}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetUnitBySMIdAsync(string smid)
        {
            try
            {
                var entity = await _services.GetUnitBySMIdAsync(smid);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get unit contract
        /// </summary>
        /// <param name="smid">ERP Unit identification Id</param>
        /// <param name="contractnumber">ERP contract identification number</param>
        /// <returns>Unit information</returns>
        /// <remarks>
        /// - Get unit information in CRM by SM id
        /// - Get full contract information in CRM by contract number.
        /// - Returns info of unit and contract
        /// </remarks>
        /// <response code = "200">Info of unit and contract</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("units/{smid}/{contractnumber}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetUnitContractAsync(string smid, string contractnumber)
        {
            try
            {
                var entity = new UnitTimeZone();
                entity.Unit = await _services.GetUnitBySMIdAsync(smid);
                ContractFull contract = await _contractService.GetFullContractAsync(contractnumber);
                // entity.TimeZone = await _contractService.GetContractTimeZoneAsync(contract.contract.SmContractCode)
                entity.TimeZone = contract.smcontract.Timezone;
                entity.StoreCoordinatesLatitude = contract.contract.StoreData.CoordinatesLatitude;
                entity.StoreCoordinatesLongitude = contract.contract.StoreData.CoordinatesLongitude;
                entity.StoreTelephone = contract.contract.StoreData.Telephone;
                entity.StoreName = contract.contract.StoreData.StoreName;
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Save image unit category
        /// </summary>
        /// <param name="document"></param>
        /// <returns>Document Id</returns>
        /// <remarks>
        /// Save the document in de Blob Storage (BlobStorageUnitImageContainer)
        /// </remarks>
        /// <response code = "200">Document Id</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPost("units/category/image")]
        public async Task<ApiResponse> UploadImageUnitCategoryAsync([FromBody] Document document)
        {
            try
            {
                var result = await _services.SaveImageUnitCategoryAsync(document);
                return new ApiResponse(null, result);
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
        /// Get list Blob content of units from CRM
        /// </summary>
        /// <param name="names">List of units</param>
        /// <returns>List of units</returns>
        /// <remarks>
        /// Get a list of Blob units (`BlobStorageUnitImageContainer`)
        /// </remarks>
        /// <response code = "200">List of units</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("units/category/image/info")]
        //[AuthorizeAzureAD(new[] { Entities.enums.RoleGroupTypes.StoreManager })]
        public async Task<ApiResponse> GetImageUnitCategoryAsync(string names)
        {
            try
            {
                var result = await _services.GetDocumentInfoBlobStorageUnitCategoryImageAsync(names);
                return new ApiResponse(null, result);
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
        /// Get all available countries from CRM
        /// </summary>
        /// <returns>Country data model list</returns>
        /// <remarks>
        /// Get a list of countries
        /// </remarks>
        /// <response code = "200">List of countries</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("countries")]
        //[AuthorizeToken]
        public async Task<ApiResponse> GetCountriesAsync()
        {
            try
            {
                var entity = await _services.GetStoresCountriesAsync();
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get cities from country from CRM
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <returns>City data model list</returns>
        /// <remarks>
        /// Get a list of cities from CRM
        /// </remarks>
        /// <response code = "200">List of cities</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("cities")]
        //[AuthorizeToken]
        public async Task<ApiResponse> GetCitiesAsync(string countryCode)
        {
            try
            {
                var entity = await _services.GetStoresCitiesAsync(countryCode);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get a store from store code
        /// </summary>
        /// <param name="storeCode">Store code</param>
        /// <returns>Store data model</returns>
        /// <remarks>
        /// Get a store from CRM by store code
        /// </remarks>
        /// <response code = "200">Info of Store</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("stores/{storeCode}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetStoreAsync(string storeCode)
        {
            try
            {
                var entity = await _services.GetStoreAsync(storeCode);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get site access code
        /// </summary>
        /// <param name="value">Access code credentials</param>
        /// <returns>Site access code</returns>
        /// <remarks>
        /// - Gets the DB user from the user logged in the system.
        /// - Logged user into the identity server
        /// - The CRM contract and the SM Contract are obtained from the SM contract id
        /// - SM contract password is retrieved
        /// - The AccessCode entity is returned with the SM user data and password
        /// </remarks>
        /// <response code = "200">Access Code information and Password form SM</response>
        /// <response code = "400">Password not valid</response> 
        /// <response code = "500">Internal Server Error</response>
        [HttpPost("access-code")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetAccessCodeAsync([FromBody] AccessCode value)
        {
            try
            {
                var entity = await _services.GetAccessCodeAsync(value.ContractId, value.Password);
                return new ApiResponse(entity);
            }
            catch (ServiceException ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
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
        /// Checks if site access code is available due to invalid password attempts
        /// </summary>
        /// <returns>bool</returns>
        /// <remarks>
        /// - Gets the DB user from the user logged in the system.
        /// - Return false if the user has exceeded the number of login attempts allowed and established by configuration
        /// </remarks>
        /// <response code = "200">True or False</response>
        /// <response code = "400">Password not valid</response> 
        /// <response code = "500">Internal Server Error</response>
        [HttpPost("access-code-available")]
        [AuthorizeToken]
        public async Task<ApiResponse> IsAccessCodeAvailableAsync()
        {
            try
            {
                var entity = await _services.IsAccessCodeAvailableAsync();
                return new ApiResponse(entity);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update access code
        /// </summary>
        /// <param name="value">Update Access code credentials</param>
        /// <returns>Bool</returns>
        /// <remarks>
        /// - Gets the DB user from the user logged in the system.
        /// - Get SM contract by SM Contract id
        /// - Gets the subcontract from SM
        /// - Update the password in SM
        /// - An email is sent to the user, in the user's language, with the EditAccessCode template
        /// - CRM contract is obtained
        /// - Mail is sent to the store, with the EditAccessCodeToStore template
        /// - Returns "true" if the password has been updated in SM, or not, returns "false"
        /// </remarks>
        /// <response code = "200">True or False</response>
        /// <response code = "400">Security access code error</response>
        /// <response code = "404">
        /// - Contract does not exist.
        /// - SubContractId does not exist.
        /// </response>
        /// <response code = "500">
        /// - Internal Server Error
        /// - Error updating accessCode.
        /// </response>
        [HttpPatch("access-code")]
        [AuthorizeToken]
        public async Task<ApiResponse> UpdateCodeAsync([FromBody] AccessCode value)
        {
            try
            {
                var entity = await _services.UpdateAccessCodeAsync(value.ContractId, value.Password);
                return new ApiResponse(entity);
            }
            catch (ServiceException ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
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
        /// Get Last user invoices
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="contractNumber">Contract Number (Optional)</param>
        /// <returns>Last n user invoices</returns>
        /// <remarks>
        /// - Retrieves from the "Features" table the number of documents to be displayed
        /// - Obtains the user by username and verifies that it exists
        /// - Obtain CRM contracts by DNI
        /// - For each CRM contract, you get the SM contract
        /// - Retrieve invoices
        /// - Returns the list of invoices grouped by contract
        /// </remarks>
        /// <response code = "200">Returns the list of invoices grouped by contract</response>
        /// <response code = "404">User does not exist.</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("invoices/{username}/{contractNumber?}")]
        public async Task<ApiResponse> GetInvoicesAsync(string username, string contractNumber = null)
        {
            try
            {
                var entity = await _services.GetLastInvoices(username, contractNumber);
                return new ApiResponse(entity);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Blob content by store code
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns>Image from Blob Storage</returns>
        /// <remarks>
        /// - The information of the image saved in the StoreImages table is obtained
        /// - The Blob Storage image is returned (BlobStorageStoreFacadeImageContainer)
        /// </remarks>
        /// <response code = "200">Image from Blob Storage</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("stores/facade/image/info")]
        public async Task<ApiResponse> GetImageStoreFacadeAsync(string storeCode)
        {
            try
            {
                var result = await _services.GetDocumentInfoStoreFacadeAsync(storeCode);
                return new ApiResponse(null, result);
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
        /// Save image unit category
        /// </summary>
        /// <param name="document">Document info</param>
        /// <param name="storeCode">Store Code</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// - Check if image data exists before saving
        /// - If they do not exist:
        /// - It is recorded on BD
        /// - uploaded to Blob Storage
        /// - The url of the image is uploaded to CRM
        /// - If data already exists:
        /// - The name in DB is updated
        /// - The old image is deleted from the BlobStorage and the new one is uploaded
        /// - The url of the image is uploaded to CRM
        /// </remarks>
        /// <response code = "200">True if the image is uploaded, false if not.</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPost("stores/facade/image")]
        public async Task<ApiResponse> UploadImageStoreFacadeAsync([FromBody] Document document, string storeCode)
        {
            try
            {
                var result = await _services.SaveImageStoreFacadeAsync(document, storeCode);
                return new ApiResponse(null, result);
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
        /// Delete image store facade
        /// </summary>
        /// <param name="storeCode">Store Code</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// - Check if image data exists
        /// - The image of the BlobStorage and the associated data of the BD are deleted
        /// - The image url field in CRM is set to null
        /// </remarks>
        /// <response code = "200">True if delete ok</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpDelete("stores/facade/image")]
        public async Task<ApiResponse> DeleteImageStoreFacadeAsync(string storeCode)
        {
            try
            {
                var result = await _services.DeleteImageStoreFacadeAsync(storeCode);
                return new ApiResponse(null, result);
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
