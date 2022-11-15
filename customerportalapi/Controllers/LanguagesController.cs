using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageServices _services;
        private readonly ILogger<LanguagesController> _logger;

        public LanguagesController(ILanguageServices services, ILogger<LanguagesController> logger)
        {
            _services = services;
            _logger = logger;
        }


        /// <summary>
        /// Get available languages
        /// </summary>
        /// <returns>List of languages</returns>        
        /// <remarks>
        /// This method call to the CRM API
        /// Returns a list of languages
        /// </remarks>
        /// <response code = "200">Return a list of languages</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet]
        public async Task<ApiResponse> GetLanguagesAsync()
        {
            try
            {
                List<Language> result = await _services.GetLanguagesAsync();
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
