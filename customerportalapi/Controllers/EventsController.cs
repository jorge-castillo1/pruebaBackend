using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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
        /// <remarks>
        /// ### This method find and update the process document. Only update document status process with status distinct of:
        /// - `document_completed`, `document_canceled` or `audit_trail_completed`
        /// ---
        /// ### If the change of payment method is made to Bank Transfer:
        /// - Search the user in DB.
        /// - Gets the information of the user's profile in CRM from the DNI.
        /// - Search the signature process by id.
        /// - Gets all CRM contract data.
        /// - Gets the email template.
        /// - Checks in CRM if the payment method for the store of type `bank` is available.
        /// - Find the account number (IBAN) from the DB `spacemanager.apsreferences` or from SM.
        /// - Update the profile in CRM with the recovered data.
        /// - Contract data is updated in CRM.
        /// - Sent mail to the store.
        /// ---
        /// ### If the change of payment method is made to Card:
        /// - Search the user in DB.
        /// - Gets the information of the user's profile in CRM from the DNI.
        /// - Search the signature process by id.
        /// - Confirm the change of payment method to card in the payment gateway.
        /// - The status of the process in DB is updated.
        /// - The card in the DB is updated.
        /// - Retrieves the data from the CRM store
        /// - Update the profile in CRM with the recovered data.
        /// - Gets all CRM contract data and updated.		
        /// - Gets the email template.       
        /// - Sent mail to the store.
        /// </remarks>
        /// <response code = "200">Return if the document exist or not</response>
        /// <response code = "400">
        /// - Document status not valid
        /// - More than one process was found
        /// - Error searching signature for this process
        /// - Store not found
        /// - Error get payment method crm
        /// - No IBAN found in Aps
        /// - Error updating account
        /// - Error updating contract
        /// </response>
        /// <response code = "404">Store mail not found</response>
        /// <response code = "500">Internal Server Error</response>
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