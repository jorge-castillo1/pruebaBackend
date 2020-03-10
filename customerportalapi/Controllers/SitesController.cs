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

        [HttpGet("users/{dni}")]
        [HttpGet("users/{dni}/Residential")]
        // [Authorize(Roles = Role.Admin)]
        public async Task<ApiResponse> GetAsync(string dni)
        {
            try
            {
                var entity = await _services.GetContractsAsync(dni, AccountType.Residential);
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

        [HttpGet("users/{dni}/Business")]
        // [Authorize(Roles = Role.Admin)]
        public async Task<ApiResponse> GetBuinessAsync(string dni)
        {
            try
            {
                var entity = await _services.GetContractsAsync(dni, AccountType.Business);
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
    }
}
