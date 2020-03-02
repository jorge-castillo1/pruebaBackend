using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;


namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IPaymentService _service;

        public EventsController(IPaymentService service, ILogger<EventsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult SignatureStatus([FromBody] SignatureStatus value)
        {
            try
            {
                _service.UpdatePaymentProcess(value);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}