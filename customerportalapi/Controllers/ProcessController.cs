using AutoWrapper.Wrappers;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using customerportalapi.Security;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessService _service;
        private readonly ILogger<ProcessController> _logger;

        public ProcessController(IProcessService service, ILogger<ProcessController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get pending process from user, contract and process type
        /// </summary>
        /// <param name="user">userName</param>
        /// <param name="smContractCode">Unique contract number from erp</param>
        /// <param name="processtype">Type of process</param>
        /// <returns>Process data or empty list</returns>
        /// <remarks>
        /// This method searches for processes in the database filtering by user, smContractCode and processtype.
        /// Returns the last process.
        /// </remarks>
        /// <response code = "200">Process data or empty list</response>        /// 
        /// <response code = "500">Internal Server Error</response>
        [HttpGet]
        public ApiResponse GetLastProcess(string user, string smContractCode = null, int? processtype = null)
        {
            try
            {
                var result = _service.GetLastProcesses(user, smContractCode, processtype);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Cancels pending process matching contract and type
        /// </summary>
        /// <param name="smContractCode">Unique contract number from erp</param>
        /// <param name="processtype">Type of process</param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code = "200"></response>
        /// <response code = "404">Process Not Found</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPut("cancel/{smContractCode}/{processtype}")]
        public ApiResponse CancelSignature(string smContractCode, int processtype)
        {
            try
            {
                var result = _service.CancelProcess(smContractCode, processtype);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Cancels all process matching username and type
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="processtype">Type of process</param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code = "200"></response>
        /// <response code = "404">Process not found</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPut("cancel/{username}/{processtype}/all")]
        public ApiResponse CancelAllProcessByUsernameAndType(string username, int processtype)
        {
            try
            {
                var result = _service.CancelAllProcessesByUsernameAndProcesstype(username, processtype);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
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