using AutoWrapper.Wrappers;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Services.Exceptions;
using customerportalapi.Security;
using customerportalapi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;
using Newtonsoft.Json;

namespace customerportalapi.Controllers
{
    /// <summary>
    /// Mail Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class MailController : ControllerBase
    {
        private readonly IMailService _service;
        private readonly ILogger<MailController> _logger;

        /// <summary>
        /// Ctor. MailController
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public MailController(IMailService service, ILogger<MailController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="email"></param>
        /// <remarks> First, this method checks in the configuration file is there any CC or CCO email 
        /// defined for the type of email that is going to be sent (EmailFlowType): 
        /// -DownloadContract 
        /// -DownloadInvoice
        /// -SendNewCredentials,
        /// -UpdatePaymentCard,
        /// -UpdateAccessCode,
        /// -UpdatePayment,
        /// -GetProfile,
        /// -UpdateProfile,
        /// -InviteUser,
        /// -SendWelcome,
        /// -UpdateAccount,
        /// -ContactCall,
        /// -ContactContact,
        /// -Contact,
        /// -SendMailInvitationError,
        /// -SaveNewUser,
        /// -ExceptionDocumetStoreApi,
        /// -SendWelcomeCronJob
        /// Example of emails defined in the configuration file by type (EmailFlowType): 
        /// - Mail**DownloadContract**CC: support.bluespace@quantion.com
        /// - Mail**DownloadContract**CCO: webportal@bluespace.eu
        /// - Mail**DownloadInvoice**CC: support.bluespace@quantion.com
        /// - Mail**DownloadInvoice**CCO: webportal@bluespace.eu
        /// It is also verified that the emails are valid.
        /// The email is sent with its recipients, subject and body of the message.
        /// </remarks>
        /// <example>**Pedro**</example>
        /// <response code = "200">Email sent</response>
        /// <response code = "500">Internal Server Error</response>

        [HttpPost]
        public ApiResponse SendEmail([FromBody] Entities.Email email)
        {
            try
            {
                var result = _service.SendEmail(email);
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