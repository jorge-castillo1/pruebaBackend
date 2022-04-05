using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities.Enums;
using customerportalapi.Security;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class WebTemplatesController : ControllerBase
    {
        private readonly IWebTemplateServices _services;
        private readonly ILogger<WebTemplatesController> _logger;

        public WebTemplatesController(IWebTemplateServices services, ILogger<WebTemplatesController> logger)
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Get customer portal list templates
        /// </summary>
        /// <returns>Template data model list</returns>
        //GET: api/templates
        [HttpGet]
        public async Task<ApiResponse> GetAsync()
        {
            try
            {
                var entity = await _services.GetTemplates();
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get customer portal template
        /// </summary>
        /// <param name="code">Template code</param>
        /// <param name="language">Template language</param>
        /// <returns>Template data model</returns>
        // GET api/templates/{language}
        [HttpGet("{code}/{language}")]
        public async Task<ApiResponse> GetAsync(int code, string language)
        {
            try
            {
                var entity = await _services.GetTemplate((WebTemplateTypes) code, language);
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
