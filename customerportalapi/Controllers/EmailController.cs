using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customerportalapi.Controllers
{
    /// <summary>
    /// Send email (use from internal webportal APIs)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController
    {
        private readonly IMailService _services;
        private readonly ILogger<EmailController> _logger;


        public EmailController(IMailService services, ILogger<EmailController> logger)
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Send email 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeApiKey]
        public async Task<ApiResponse> Send([FromBody] Email email)
        {
            try
            {
                var entity = await _services.Send(email);
                return new ApiResponse(entity);
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (email != null)
                    obj = ", params:" + JsonConvert.SerializeObject(email);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }
    }
}
