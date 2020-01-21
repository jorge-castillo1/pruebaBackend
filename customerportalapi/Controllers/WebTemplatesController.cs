using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities.enums;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebTemplatesController : ControllerBase
    {
        private readonly IWebTemplateServices _services;
        private readonly ILogger<WebTemplatesController> _logger;

        public WebTemplatesController(IWebTemplateServices services, ILogger<WebTemplatesController> logger)
        {
            _services = services;
            _logger = logger;
        }

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
