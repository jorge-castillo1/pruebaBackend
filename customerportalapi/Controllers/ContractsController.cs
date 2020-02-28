using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class ContractsController : ControllerBase
    {
        private readonly IContractServices _services;
        private readonly ILogger<SitesController> _logger;


        public ContractsController(IContractServices services, ILogger<SitesController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [HttpGet("{contractNumber}")]
        public async Task<ApiResponse> GetAsync(string contractNumber)
        {
            try
            {
                var entity = await _services.GetContractAsync(contractNumber);
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

        [HttpGet("full/{contractNumber}")]
        public async Task<ApiResponse> GetFullContractAsync(string contractNumber)
        {
            try
            {
                var entity = await _services.GetFullContractAsync(contractNumber);
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

        [HttpGet("{contractNumber}/download")]
        public async Task<ApiResponse> GetDownloadContractAsync(string contractNumber)
        {
            try
            {
                var entity = await _services.GetDownloadContractAsync(contractNumber);
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
