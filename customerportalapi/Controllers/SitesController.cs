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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// Get Blob content
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
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
        /// Get all available countries
        /// </summary>
        /// <returns>Country data model list</returns>
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
        /// Get cities from country
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <returns>City data model list</returns>
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
        /// Get stores from store code
        /// </summary>
        /// <param name="storeCode">Store code</param>
        /// <returns>Store data model</returns>
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
        [HttpGet("invoices/{username}/{contractNumber?}")]
        public async Task<ApiResponse> GetInvoicesAsync(string username, string contractNumber = null)
        {
            try
            {
                var entity = await _services.GetLastDocuments(username, contractNumber);
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
        /// <returns></returns>
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
        /// <param name="document"></param>
        /// <param name="storeCode"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
