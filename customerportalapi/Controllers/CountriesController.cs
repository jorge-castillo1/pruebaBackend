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
