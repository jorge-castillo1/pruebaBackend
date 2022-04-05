using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Entities;
using customerportalapi.Services.Interfaces;
using customerportalapi.Entities.Enums;
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
                _logger.LogInformation($"EventsController.SignatureStatus(value). value: {JsonConvert.SerializeObject(value)}.");

                Process process = _service.UpdateDocumentStatusProcess(value);

                _logger.LogInformation($"EventsController.SignatureStatus(value). UpdateSignatureProcess. Returned process: {JsonConvert.SerializeObject(process)}.");

                if (process != null)
                {
                    if (process.ProcessType == (int)ProcessTypes.PaymentMethodChangeBank && process.ProcessStatus == (int)ProcessStatuses.Accepted)
                    {
                        _logger.LogInformation($"EventsController.SignatureStatus(value). ProcessType == PaymentMethodChangeBank && ProcessStatuses == Accepted: UpdatePaymentProcess(value).");
                        await _paymentService.UpdatePaymentBankProcess(value);
                    }

                    if (process.ProcessType == (int)ProcessTypes.PaymentMethodChangeCardSignature && process.ProcessStatus == (int)ProcessStatuses.Accepted)
                    {
                        _logger.LogInformation($"EventsController.SignatureStatus(value). ProcessType == PaymentMethodChangeCardSignature && ProcessStatuses == Accepted: UpdatePaymentCardProcess(value, process).");
                        await _paymentService.UpdatePaymentCardProcess(value, process);
                    }
                }

                _logger.LogInformation($"EventsController.SignatureStatus(value). Return OkResult()");
                return new OkResult();
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(ex, $"EventsController.SignatureStatus(value). {ex.Message}.{Environment.NewLine}value: {obj}.");
                throw;
            }
        }
    }
}