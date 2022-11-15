using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Security;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryServices _services;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountryServices services, ILogger<CountriesController> logger)
        {
            _services = services;
            _logger = logger;
        }


        /// <summary>
        /// Get available countries
        /// </summary>
        /// <returns>List of Countries</returns>
        /// <remarks>
        /// This method call to the CRM API
        /// Returns a list of countries order by country name
        /// </remarks>
        /// <response code = "200">Return a list of countries</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet]
        public async Task<ApiResponse> GetAsync()
        {
            try
            {
                var result = await _services.GetCountriesAsync();
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
