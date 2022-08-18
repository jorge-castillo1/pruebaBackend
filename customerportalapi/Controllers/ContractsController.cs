using AutoWrapper.Wrappers;
using customerportalapi.Entities;
using customerportalapi.Security;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        /// <param name="contractNumber">friendly user contract number</param>
        /// <returns>FullContract model data</returns>
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
        [HttpPost]
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
        /// Obtain if document exists in sharepoint from contract number
        /// </summary>
        /// <param name="smContractCode">friendly user contract number</param>
        /// <returns>boolean</returns>
        [HttpGet("document/{smContractCode}/exists")]
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
        /// Obtain if invoice exists in sharepoint from contract number
        /// </summary>
        /// <param name="invoiceRequest">friendly user contract number</param>
        /// <returns>boolean</returns>
        [HttpPost("invoice/exists")]
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
        /// <returns>bolean</returns>
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

        /// <summary>
        /// Update Signature Id in CRM Contracts
        /// </summary>
        /// <returns>Contract info of Signaturit</returns>
        [HttpPost("UpdateSignatureId")]
        public async Task<ApiResponse> UpdateContractsWithoutSignatureId(string fromCreatedOn, string toCreatedOn = null, string arrContracts = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fromCreatedOn))
                    return new ApiResponse(400, "Params not valid");

                var entity = await _services.UpdateContractsWithoutSignatureId(fromCreatedOn, toCreatedOn, arrContracts);
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
        /// Upload Documents in Sharepoint by ContractId
        /// </summary>
        /// <param name="arrContracts"></param>
        /// <returns></returns>
        [HttpPatch("UploadDocuments")]
        public async Task<ApiResponse> UploadDocuments(string arrContracts)
        {
            try
            {
                if (string.IsNullOrEmpty(arrContracts))
                    return new ApiResponse(400, "Params not valid");

                var entity = await _services.UploadDocuments(arrContracts);
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