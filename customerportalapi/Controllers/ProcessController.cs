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

        [HttpGet]
        public ApiResponse GetLastProcess(string user, string contractnumber = null, int? processtype = null)
        {
            try
            {
                var result = _service.GetLastProcesses(user, contractnumber, processtype);
                return new ApiResponse(result);
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

        [HttpPut("cancel/{contractnumber}/{processtype}")]
        public ApiResponse CancelSignature(string contractnumber, int processtype)
        {
            try
            {
                var result = _service.CancelProcess(contractnumber, processtype);
                return new ApiResponse(result);
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