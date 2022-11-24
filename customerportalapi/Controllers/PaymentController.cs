using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Loggers;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


        /// <summary>
        /// Change of payment method to Bank
        /// </summary>
        /// <param name="value">Info of new payment method</param>
        /// <returns>True or False</returns> 
        /// <remarks>
        /// ## This method updated payment method to Bank.
        /// - Get the user from DB by DNI and it is verified that it exists.
        /// - Verify that there are no pending processes for this user and contract.
        /// - Update the data of the payment method(APS) in SM.
        /// - Generates a document(form) and sends it to Signaturit.
        /// - Updates the user's profile with the `TokenUpdate = 1` (Pending) and the date.
        /// - Record the data of the payment method change in the Process table.
        /// </remarks>
        /// <response code = "200">Return if the change is ok or not</response>
        /// <response code = "400">
        /// - Contract number field can not be null.
        /// - User have same pending process for this contract number. 
        /// </response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);

                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Change of payment method to Card
        /// </summary>
        /// <param name="value">Info of new payment method</param>
        /// <returns>True or False</returns> 
        /// <remarks>
        /// ## This method updated payment method to Card.
        /// - Get the user from DB by `username` and it is verified that it exists.
        /// - Check in BD (CARD table) that there is another card with that `ExternalID`.
        /// - Get CRM Profile.
        /// - Verify that there are no pending processes for this user and contract.
        /// - Generates a document(form) and sends it to Signaturit.
        /// - Record the data of the payment method change in the Process table and in the Card table.
        /// </remarks>
        /// <response code = "200">Return if the change is ok or not</response>
        /// <response code = "400">
        /// - Contract number field can not be null.
        /// - Card not found.
        /// - User have two or more started process for this externalId and ProcessType.
        /// - User don't have started process for this externalId and ProcessType.
        /// </response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Change of payment method
        /// </summary>
        /// <param name="value">Info of new payment method</param>
        /// <returns>True or False</returns>
        /// <remarks>
        /// - Get the user from DB by DNI and it is verified that it exists.
        /// - Get CRM Profile.
        /// - Assigns the IdCustomer of SM.
        /// - New ExternalId is generated.
        /// - It is verified that the information received in all the fields is correct.
        /// - The existing payment process in BD and in Precognis is canceled.
        /// - A new form with the payment data is sent to Precognis.
        /// - Record the data of the payment method change in the Process table and in the Card table.
        /// </remarks>
        /// <response code = "200">Return if the change is ok or not</response>
        /// <response code = "400">
        /// - Site Id field can not be null
        /// - Contract number field can not be null            
        /// - Email field can not be null            
        /// - Email field must not be longer to 254.            
        /// - Phone number can not be null.            
        /// - Phone number field must not be longer to 15.
        /// - Street1 can not be null.
        /// - Street1 field must not be longer to 50.
        /// - Street2 field must not be longer to 50.
        /// - Street3 field must not be longer to 50.
        /// - Zip Or Postal Code can not be null.
        /// - Zip Or Postal Code field must not be longer to 16.
        /// - City can not be null.
        /// - City field must not be longer to 50.
        /// - Country ISO Code Numeric can not be null.
        /// - Country ISO Code Numeric field must not be longer to 3.
        /// - Phone Prefix can not be null.
        /// - Phone Prefix field must not be longer to 3.
        /// - Id Customer can not be null. 
        /// </response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Response of a change of payment method
        /// </summary>
        /// <param name="value">Info of new payment method</param>
        /// <returns>True or False</returns>
        /// <remarks>
        /// - Card information is updated in BD
        /// - Verify that there are no pending processes for this card
        /// - If the Status is different from "00", the information of the process in DB is updated and an exception is returned
        /// - If it is "00", Gets the information from the user's payment method, from CRM and from the information by parameter
        /// - And execute the process of change the payment method.
        /// </remarks>
        /// <response code = "200">Return if the response is ok or not</response>
        /// <response code = "400">
        /// - Card doesn´t exist.
        /// - User have two or more started process for this externalId.
        /// - Error card verification.
        /// </response>
        /// <response code = "404">
        /// - User does not exist.
        /// - Contract does not exist.</response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Get info of Card
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="smContractCode">Contract Code of SM</param>
        /// <returns>Info of Card</returns>
        /// <remarks>Card information is obtained of Precognis by the token.</remarks>
        /// <response code = "200">Info of Card</response>
        /// <response code = "400">Card data error</response>
        /// <response code = "500">Internal Server Error</response>
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
        /// Paying an invoice
        /// </summary>
        /// <param name="value">Info of payment</param>
        /// <returns>Info of Payment</returns>
        /// <remarks>
        /// ### Paying an invoice
        /// - All SM invoices are obtained by the SMContractCode.
        /// - It is filtered by the invoice in question.
        /// - Obtains SM contract info.
        /// - The payment is sent to Precognis.
        /// - Obtains the contract and the store from CRM.
        /// - Obtains the CRM payment method
        /// - Payment is recorded in SM
        /// - An email is sent to the store, in the language of the store, with the RegisteredInvoicePayment template.
        /// </remarks>
        /// <response code = "200">Info of Payment</response>
        /// <response code = "400">
        /// - SiteId is required
        /// - SmContractCode is required
        /// - Ourref is required
        /// - Token is required
        /// - Username is required
        /// - Invoices not found
        /// - Contract sm found
        /// - Error payment
        /// - Store not found
        /// - Error payment method crm
        /// </response>
        /// <response code = "404">Store mail not found. RegisteredInvoicePayment.</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPost]
        [Route("invoice")]
        [AuthorizeToken]
        [CustomLog]
        public async Task<ApiResponse> PayInvoice(PaymentMethodPayInvoice value)
        {
            try
            {
                PaymentMethodPayInvoiceResponse entity = await _services.PayInvoice(value);
                return new ApiResponse(entity);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Pay invoice in Precognis with new Card
        /// </summary>
        /// <param name="value"></param>
        /// <returns>HTML with the response of payment</returns>
        /// <remarks>
        /// - The database user is obtained by the username.
        /// - Store and profile information is obtained in CRM.
        /// - Contract and the invoices information is obtained of SM
        /// - It is filtered by the invoice in question.
        /// - The information in the payment method fields is verified.
        /// - The payment process is canceled.
        /// - The payment of the invoice is made with a new card.
        /// - The Payment(Pay) is created and in Process in the DB.
        /// - The payment response HTML is returned to Precognis.
        /// </remarks>
        /// <response code = "200">HTML with the response of payment</response>
        /// <response code = "400">
        /// -Contract number field can not be null.
        /// -Invoices not found
        /// </response>
        /// <response code = "404">User does not exist.</response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Response of Pay invoice with new Card
        /// </summary>
        /// <param name="value">Info of payment</param>
        /// <returns>True or False</returns>
        /// <remarks>
        /// - The payment information in BD is updated
        /// - Verify that there are no pending processes for this user and contract.
        /// - If the payment status is different from "00", the data in the Process table is updated and an exception is returned.
        /// - Otherwise:
        /// - Contract information is obtained in CRM
        /// - The payment method information is obtained in CRM
        /// - You get the invoice in SM
        /// - Payment is made in SM
        /// - The process is recorded
        /// - An email is sent to the store, in the language of the store with the RegisteredInvoicePayment template
        /// </remarks>
        /// <response code = "200">True is ok and False if not.</response>
        /// <response code = "400">
        /// - Pay doesn´t exist
        /// - User have two or more started process for this externalId
        /// - User don't have started process for this externalId &amp; ProcessType
        /// - Payment error
        /// - Store not found
        /// - Error payment method crm
        /// - Invoices not found
        /// - Store mail not found. RegisteredInvoicePayment.
        /// </response>
        /// <response code = "500">Internal Server Error</response>
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
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Update payment in Precognis
        /// </summary>
        /// <param name="value">Info of payment</param>
        /// <returns>HTML form response of payment in Precognis</returns>
        /// <remarks>
        /// - Get the user from DB by `username` and it is verified that it exists.
        /// - Get CRM Account.
        /// - Validate data.
        /// - Cancel Process of payment in Precognis
        /// - Update the Card in Precognis
        /// - Update the Account in CRM
        /// - Create the Card info in DB
        /// - Create the Process info in DB
        /// - Returns HTML form response of payment in Precognis
        /// </remarks>
        /// <response code = "200">HTML form response of payment in Precognis</response>
        /// <response code = "400">
        /// Contract number field can not be null.
        /// Error creating car.
        /// </response>
        /// <response code = "404">User does not exist</response>
        /// <response code = "500">Internal Server Error</response>
        //POST api/payment/update-card/load
        [HttpPost]
        [Route("update-card/load")]
        [AuthorizeToken]
        public async Task<ApiResponse> UpdateCardLoad([FromBody] PaymentMethodUpdateCardData value)
        {
            try
            {
                var result = await _services.UpdateCardLoad(value);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Response of Update Card
        /// </summary>
        /// <param name="value">Info of payment</param>
        /// <returns>True or False</returns>
        /// <remarks>
        /// - The card information is obtained from the DB and updated with the new information.
        /// - Verify that there are no pending processes for this user and contract.
        /// - The payment method change process is executed.
        /// </remarks>
        /// <response code = "200">True if ok and False if not.</response>
        /// <response code = "400">
        /// - Card doesn´t exist
        /// - User have two or more started process for this externalId
        /// - Error card verification
        /// </response>
        /// <response code = "500">Internal Server Error</response>
        //POST api/payment/update-card/response
        [HttpPost]
        [Route("update-card/response")]
        public async Task<ApiResponse> UpdateCardResponse([FromBody] PaymentMethodUpdateCardResponse value)
        {
            try
            {
                var result = await _services.UpdateCardResponseAsync(value);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (value != null)
                    obj = ", params:" + JsonConvert.SerializeObject(value);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
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

        /// <summary>
        /// Get availabe payments methods by SM Contract Code
        /// </summary>
        /// <param name="smContractCode">SM Contract Code</param>
        /// <returns>List of Payment Methods</returns>
        /// <remarks>
        /// - Get the contract and the store info in CRM
        /// - Get the store info
        /// - Gets the associated payment methods by the store code
        /// </remarks>
        /// <response code = "200">List of Payments Methods</response>
        /// <response code = "400">
        /// - Store not found
        /// - Error payment method crm
        /// </response>
        /// <response code = "404">Contract does not exist.</response>
        /// <response code = "500">Internal Server Error</response>
        //GET api/payment/availablepaymentmethods/{smContractCode}
        [HttpGet("availablepaymentmethods/{smContractCode}")]
        public async Task<ApiResponse> GetAvailablePaymentMethods(string smContractCode)
        {
            try
            {
                List<PaymentMethods> entity = await _services.GetAvailablePaymentMethods(smContractCode);
                return new ApiResponse(entity);
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
