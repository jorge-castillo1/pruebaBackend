using AutoWrapper.Wrappers;
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
        [AuthorizeToken]
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

        //POST api/payment/changepaymentmethod/card
        [HttpPost]
        [Route("changepaymentmethod/card")]
        [AuthorizeToken]
        public async Task<ApiResponse> ChangePaymentMethodCardAsync([FromBody] PaymentMethodCardSignature value)
        {
            try
            {
                value.PaymentMethodType = (int)PaymentMethodTypes.CreditCard;
                var result = await _services.ChangePaymentMethodCard(value);
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

        //POST api/payment/changepaymentmethod/card/load
        [HttpPost]
        [Route("changepaymentmethod/card/load")]
        [AuthorizeToken]
        public async Task<ApiResponse> ChangePaymentMethodCardLoadAsync([FromBody] PaymentMethodCard value)
        {
            try
            {
                value.PaymentMethodType = (int)PaymentMethodTypes.CreditCard;
                var result = await _services.ChangePaymentMethodCardLoad(value);
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

        //POST api/payment/changepaymentmethod/card/response
        [HttpPost]
        [Route("changepaymentmethod/card/response")]
        public async Task<ApiResponse> ChangePaymentMethodCardResponseAsync([FromBody] PaymentMethodCardData value)
        {
            try
            {
                var result = await _services.ChangePaymentMethodCardResponseAsync(value);
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

        //GET api/payment/cards/{username}/{smContractCode}
        [HttpGet("cards/{username}/{smContractCode}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetCard(string username, string smContractCode)
        {
            try
            {
                Card entity = await _services.GetCard(username, smContractCode);
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

        [HttpPost]
        [Route("invoice")]
        [AuthorizeToken]
        public async Task<ApiResponse> PayInvoice(PaymentMethodPayInvoice payInvoice)
        {
            try
            {
                PaymentMethodPayInvoiceResponse entity = await _services.PayInvoice(payInvoice);
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

        //POST api/payment/invoice/load
        [HttpPost]
        [Route("invoice/load")]
        [AuthorizeToken]
        public async Task<ApiResponse> PayInvoiceByNewCardLoad([FromBody] PaymentMethodPayInvoiceNewCard value)
        {
            try
            {
                var result = await _services.PayInvoiceByNewCardLoad(value);
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

        //POST api/payment/invoice/response
        [HttpPost]
        [Route("invoice/response")]
        public async Task<ApiResponse> PayInvoiceByNewCardResponse([FromBody] PaymentMethodPayInvoiceNewCardResponse value)
        {
            try
            {
                var result = await _services.PayInvoiceByNewCardResponse(value);
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
