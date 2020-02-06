using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<SitesController> _logger;


        public SitesController(ISiteServices services, ILogger<SitesController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("users/{dni}")]
        // [Authorize(Roles = Role.Admin)]
        public async Task<ApiResponse> GetAsync(string dni)
        {
            try
            {
                var entity = await _services.GetContractsAsync(dni);
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
        public async Task<ApiResponse> GetAsync(string countryCode, string city)
        {
            try
            {
                var entity = await _services.GetStoresAsync(countryCode, city);
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
    }
}
