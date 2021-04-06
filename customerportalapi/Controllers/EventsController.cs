using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using customerportalapi.Services.Interfaces;
using customerportalapi.Entities.enums;
using Newtonsoft.Json;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IProcessService _service;
        private readonly IPaymentService _paymentService;

        public EventsController(IProcessService service, IPaymentService paymentservice, ILogger<EventsController> logger)
        {
            _service = service;
            _paymentService = paymentservice;
            _logger = logger;
        }

        /// <summary>
        /// Receive changes on signature process status
        /// </summary>
        /// <param name="value">Signature status data</param>
        /// <returns>Ok</returns>
        [HttpPost]
        public async Task<ActionResult> SignatureStatus([FromBody] SignatureStatus value)
        {
            try
            {
                Process process = _service.UpdateSignatureProcess(value);

                if (process != null)
                {
                    if (process.ProcessType == (int)ProcessTypes.PaymentMethodChangeBank && process.ProcessStatus == (int)ProcessStatuses.Accepted)
                    {
                        await _paymentService.UpdatePaymentProcess(value);
                    }
                
                    if (process.ProcessType == (int)ProcessTypes.PaymentMethodChangeCardSignature && process.ProcessStatus == (int)ProcessStatuses.Accepted)
                    {
                        await _paymentService.UpdatePaymentCardProcess(value, process);
                    }
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }
    }
}