﻿using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _services;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService services, ILogger<PaymentController> logger)
        {
            _services = services;
            _logger = logger;
        }

        //POST api/payment/changepaymentmethod/bank
        [HttpPost]
        [Route("changepaymentmethod/bank")]
        public async Task<ApiResponse> ChangePaymentMethodBankAsync([FromBody] PaymentMethodBank value)
        {
            try
            {
                value.PaymentMethodType = (int)PaymentMethodTypes.Bank;
                var result = await _services.ChangePaymentMethod(value);
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
