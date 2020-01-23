﻿using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Services.Exceptions;
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
    }
}
