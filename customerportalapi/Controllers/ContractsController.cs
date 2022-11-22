﻿using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractServices _services;
        private readonly ILogger<ContractsController> _logger;


        public ContractsController(IContractServices services, ILogger<ContractsController> logger)
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Obtain a contract from its contract number
        /// </summary>
        /// <param name="contractNumber">Contract number</param>
        /// <returns>Contract data model</returns>
        /// <remarks>
        /// This method calls the CRM API with the SM contract code, and gets the contract.
        /// </remarks>
        /// <response code = "200">Return CRM Contract</response>
        /// <response code = "500">Internal Server Error</response>
        /// <response code = "404">Handled error: Contract does not exist</response>
        [HttpGet("{contractNumber}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetAsync(string contractNumber)
        {
            try
            {
                var entity = await _services.GetContractAsync(contractNumber);
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
        /// Download contract
        /// </summary>
        /// <param name="dni">user document identification number</param>
        /// <param name="smContractCode">unique contract number from erp</param>
        /// <returns>base64 string document</returns>
        /// <remarks>
        /// This method makes a call to the crm API through the dni and the SM contract code returns a list of documents.
        /// Calls the CRM API with the SM contract code, and gets the contract.
        /// Sends mail with the template RequestDigitalContract to the store in the language of the store.
        /// From the id of the CRM document, a call is made to the Documents API to obtain the document in base64 string.
        /// </remarks>
        /// <response code = "200">Return a document in base64 format</response>
        /// <response code = "500">Error of type Internal Server Error</response>
        /// <response code = "400">Handled error: More than one document was found</response>
        /// <response code = "404">Handled errors:
        /// - Email Template not exist
        /// - Store mail not found
        /// - Contract file does not exist
        /// - Contract does not exist
        /// </response>
        [HttpGet("{dni}/{smContractCode}/download")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetDownloadContractAsync(string dni, string smContractCode)
        {
            try
            {
                var entity = await _services.GetDownloadContractAsync(dni, smContractCode);
                return new ApiResponse(null, entity);
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
        /// Download invoice document
        /// </summary>
        /// <param name="invoiceDownload">Invoice information metadata</param>
        /// <returns>base64 string document</returns>
        /// <remarks>
        /// This method makes a call to the crm API filter by the Invoice number and return a list of Invoice (DocumentMetadata)
        /// Sends mail with the template RequestDigitalContract to the store in the language of the store.
        /// From the first id of the CRM document, call to the Documents API to obtain the document in base64 string.
        /// </remarks>
        /// <response code = "200">Return a document in base64 format</response>
        /// <response code = "400">Handled error: More than one document was found</response>
        /// <response code = "404">Handled errors:
        /// - Email Template not exist
        /// - Store mail not found
        /// - Invoice file does not exist
        /// </response>
        /// <response code = "500">Error of type Internal Server Error</response>
        [HttpPost("invoices/download")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetDownloadInvoiceAsync([FromBody] InvoiceDownload invoiceDownload)
        {
            try
            {
                var entity = await _services.GetDownloadInvoiceAsync(invoiceDownload);
                return new ApiResponse(null, entity);
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
        /// Obtain extended contract information from contract number
        /// </summary>
        /// <param name="contractNumber">SM contract number</param>
        /// <returns>FullContract model data</returns>
        /// <remarks>
        /// This method calls the CRM API with the SM contract code, and gets extented information about the contract.
        /// This extended information includes the contract data with the TotalPrice, the Opportunity (with Expected move in, ... ) and payment methods.
        /// </remarks>
        /// <response code = "200">Return CRM Contract</response>
        /// <response code = "500">Internal Server Error</response>
        /// <response code = "404">Handled error: Contract does not exist</response>
        [HttpGet("full/{contractNumber}")]
        [AuthorizeToken]
        public async Task<ApiResponse> GetFullContractAsync(string contractNumber)
        {
            try
            {
                var entity = await _services.GetFullContractAsync(contractNumber);
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
        /// Upload contract document to document repository
        /// </summary>
        /// <param name="document">Document content and metadata</param>
        /// <returns>Unique document identification number</returns>
        /// <remarks>
        /// This method calls the DOC-API with the document and metadata information.
        /// Obtains the contract from the CRP-API by the SM Contract Code.
        /// Then updates the Contract in the CRM-API with a new composed ContractUrl.
        /// </remarks>
        /// <response code = "200">Upload succesfull</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPost]
        //[AuthorizeAzureAD(new[] { Entities.enums.RoleGroupTypes.StoreManager })]
        public async Task<ApiResponse> UploadContractAsync([FromBody] Document document)
        {
            try
            {
                var result = await _services.SaveContractAsync(document);
                return new ApiResponse(null, result);
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
        /// Obtain if document exists in sharepoint from SM contract number
        /// </summary>
        /// <param name="smContractCode">SM Contract number</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// This method call to the CRM API by the SM contract code and returns a list of contracts
        /// Filter with the type document = "0" (Contract).
        ///
        /// List of document types:
        /// - SEPA = 1
        /// - Contract = 0
        /// - Card = 2
        /// - Invoice = 3
        ///  
        /// If document exist return "true", if not it returns "false".
        /// </remarks>
        /// <response code = "200">Return if the document exist or not</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("document/{smContractCode}/exists")]
        //[AuthorizeToken]
        public async Task<ApiResponse> DocumentExists(string smContractCode)
        {
            try
            {
                var entity = await _services.DocumentExists(smContractCode);
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
        /// Obtain if invoice exists in sharepoint from SM contract number
        /// </summary>
        /// <param name="invoiceRequest">SM Contract number</param>
        /// <returns>Boolean</returns>
        /// <remarks>
        /// This method call to the CRM API by the SM contract code and returns a list of documents
        /// Filter with the type document = "3" (Invoice)
        ///
        /// List of document types:
        /// - SEPA = 1
        /// - Contract = 0
        /// - Card = 2
        /// - Invoice = 3
        /// 
        /// If document exist return "true", if not it returns "false".
        /// </remarks>
        /// <response code = "200">Return  if invoice exists or not</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpPost("invoice/exists")]
        //[AuthorizeToken]
        public async Task<ApiResponse> InvoiceExists([FromBody] InvoiceRequest invoiceRequest)
        {
            try
            {
                var entity = await _services.InvoiceExists(invoiceRequest.InvoiceNumber);
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
        /// Update Contracts Urls in CRM
        /// </summary>
        /// <returns>Object with the info of contracts updated</returns>
        /// <remarks>
        /// This method get the contracts in the CRM API (with optional limit of contracts and pagination)
        /// For each contract, the fields of the ContractUrl are updated
        /// </remarks>
        /// <response code = "200">Object with the info of contracts updated</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("UpdateContractsUrl")]
        public async Task<ApiResponse> UpdateContractUrlAsync(int? skip, int? limit)
        {
            try
            {
                var entity = await _services.UpdateContractUrlAsync(skip, limit);
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
