using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Entities;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult SignatureStatus([FromBody] SignatureStatus value)
        {
            try
            {
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