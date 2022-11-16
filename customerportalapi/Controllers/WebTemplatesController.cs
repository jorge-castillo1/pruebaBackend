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
        /// <remarks>This method searches the database for templates</remarks>
        /// <remarks>Then returns a list of templates</remarks>
        /// <response code = "200">Return a list of templates</response>
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
        /// <remarks>This method searches the database for an specific template</remarks>
        /// <remarks>It is required the code of the template and the language</remarks>
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
