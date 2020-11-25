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
    [AuthorizeToken]
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
        // [Authorize(Roles = Role.Admin)]
        public async Task<ApiResponse> GetAsync(string username)
        {
            try
            {
                var entity = await _services.GetContractsAsync(username);
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
        /// Search stores by country and/or city
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <param name="city">City</param>
        /// <param name="skip">page number</param>
        /// <param name="limit">page size</param>
        /// <returns></returns>
        [HttpGet("stores")]
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
        /// Get all available countries
        /// </summary>
        /// <returns>Country data model list</returns>
        [HttpGet("countries")]
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
        public async Task<ApiResponse> GetAccessCodeAsync([FromBody] AccessCode value)
        {
            try
            {
                var entity = await _services.GetAccessCodeAsync(value.ContractId, value.Password);
                return new ApiResponse(entity);
            }
            catch(ServiceException ex)
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
        /// Checks if site access code is available due to invalid password attempts
        /// </summary>
        /// <returns>bool</returns>
        [HttpPost("access-code-available")]
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
        public async Task<ApiResponse> UpdateCodeAsync([FromBody] AccessCode value)
        {
            try
            {
                var entity = await _services.UpdateAccessCodeAsync(value.ContractId, value.Password);
                return new ApiResponse(entity);
            }
            catch(ServiceException ex)
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
        /// Get Last user invoices
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Last n user invoices</returns>
        [HttpGet("invoices/{username}")]
        public async Task<ApiResponse> GetInvoicesAsync(string username)
        {
            try
            {
                var entity = await _services.GetLastInvoices(username);
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
    }
}
