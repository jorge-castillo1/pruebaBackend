using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ISiteServices _services;
        private readonly ILogger<SitesController> _logger;


        public SitesController(ISiteServices services, ILogger<SitesController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("{dni}")]
        public async Task<ApiResponse> GetAsync(string dni)
        {
            try
            {
                var entity = await _services.GetContractsAsync(dni);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        public async Task<ApiResponse> GetAsync(string country, string city)
        {
            try
            {
                var entity = await _services.GetStoresAsync(country, city);
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
        public async Task<ApiResponse> GetCitiesAsync(string city)
        {
            try
            {
                var entity = await _services.GetStoresCitiesAsync(city);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpGet("stores/{storeId}")]
        public async Task<ApiResponse> GetStoreAsync(string storeId)
        {
            try
            {
                var entity = await _services.GetStoreAsync(storeId);
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
