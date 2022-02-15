using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Entities.Mappers;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class ContractServices : IContractServices
    {
        private readonly IConfiguration _configuration;
        private readonly IContractRepository _contractRepository;
        private readonly IContractSMRepository _contractSMRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IOpportunityCRMRepository _opportunityRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IConfiguration _config;
        private readonly ILogger<ContractServices> _logger;


        public ContractServices(
            IConfiguration configuration,
            IContractRepository contractRepository,
            IContractSMRepository contractSMRepository,
            IMailRepository mailRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IDocumentRepository documentRepository,
            IUserRepository userRepository,
            IStoreRepository storeRepository,
            IOpportunityCRMRepository opportunityRepository,
            IPaymentMethodRepository paymentMethodRepository,

            ILogger<ContractServices> logger
        )
        {
            _configuration = configuration;
            _contractRepository = contractRepository;
            _contractSMRepository = contractSMRepository;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _opportunityRepository = opportunityRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _logger = logger;
        }

        public async Task<Contract> GetContractAsync(string contractNumber)
        {
            Contract contract = await _contractRepository.GetContractAsync(contractNumber);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, FieldNames.ContractNumber, ValidationMessages.NotExist);

            return contract;
        }

        public async Task<string> GetDownloadContractAsync(string dni, string smContractCode)
        {
            DocumentMetadataSearchFilter filter = new DocumentMetadataSearchFilter()
            {
                SmContractCode = smContractCode,
                AccountDni = dni,
                DocumentType = (int)DocumentTypes.Contract
            };
            List<DocumentMetadata> list = await _documentRepository.Search(filter);

            if (list.Count > 1) throw new ServiceException("More than one document was found", HttpStatusCode.BadRequest);
            else if (list.Count == 0)
            {
                var contract = await GetContractAsync(smContractCode);

                EmailTemplate requestDigitalContractTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.RequestDigitalContract, LanguageTypes.en.ToString());

                if (string.IsNullOrEmpty(requestDigitalContractTemplate._id))
                {
                    string errorMessage = (int)EmailTemplateTypes.RequestDigitalContract + " : " + EmailTemplateTypes.RequestDigitalContract.ToString() + " : " + LanguageTypes.en.ToString();
                    throw new ServiceException("Email Template not exist, " + errorMessage, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotExist);
                }

                Email message = new Email();
                message.EmailFlow = EmailFlow.DownloadContract.ToString();
                string mailTo = contract.StoreData.EmailAddress1;
                if (mailTo == null)
                    throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);

                if (!(_configuration["Environment"] == nameof(EnvironmentTypes.PRO))) mailTo = _configuration["MailStores"];
                message.To.Add(mailTo);
                message.Subject = string.Format(requestDigitalContractTemplate.subject, contract.Customer, dni);
                // TODO: When we will implement client new template
                // string htmlbody = requestDigitalContractTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(requestDigitalContractTemplate.body, contract.Customer, dni, contract.ContractNumber);
                await _mailRepository.Send(message);

                throw new ServiceException("Contract file does not exist, ContractNumber: " + smContractCode, HttpStatusCode.NotFound, FieldNames.ContractNumber, ValidationMessages.NotExist);
            }

            string documentId = list[0].DocumentId;

            return await _documentRepository.GetDocumentAsync(documentId);
        }
        public async Task<string> GetDownloadInvoiceAsync(InvoiceDownload invoiceDownload)
        {
            DocumentMetadataSearchFilter filter = new DocumentMetadataSearchFilter()
            {
                InvoiceNumber = invoiceDownload.InvoiceNumber,
                DocumentType = (int)DocumentTypes.Invoice
            };
            List<DocumentMetadata> list = await _documentRepository.Search(filter);

            if (list.Count > 1) throw new ServiceException("More than one document was found", HttpStatusCode.BadRequest);
            else if (list.Count == 0)
            {

                User user = _userRepository.GetCurrentUserByUsername(invoiceDownload.Username);
                Store store = await _storeRepository.GetStoreAsync(invoiceDownload.StoreCode);

                EmailTemplate requestDigitalInvoiceTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.RequestDigitalInvoice, LanguageTypes.en.ToString());

                if (string.IsNullOrEmpty(requestDigitalInvoiceTemplate._id))
                {
                    string errorMessage = (int)EmailTemplateTypes.RequestDigitalInvoice + " : " + EmailTemplateTypes.RequestDigitalInvoice.ToString() + " : " + LanguageTypes.en.ToString();
                    throw new ServiceException("Email Template not exist, " + errorMessage, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotExist);
                }

                Email message = new Email();
                message.EmailFlow = EmailFlow.DownloadInvoice.ToString();
                string mailTo = store.EmailAddress1;
                if (mailTo == null)
                    throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);

                if (!(_configuration["Environment"] == nameof(EnvironmentTypes.PRO))) mailTo = _configuration["MailStores"];
                message.To.Add(mailTo);
                message.Subject = string.Format(requestDigitalInvoiceTemplate.subject, user.Name, user.Dni, invoiceDownload.InvoiceNumber);
                // TODO: When we will implement client new template
                // string htmlbody = requestDigitalInvoiceTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(requestDigitalInvoiceTemplate.body, user.Name, user.Dni, invoiceDownload.InvoiceNumber);
                await _mailRepository.Send(message);

                throw new ServiceException("Invoice file does not exist, InvoiceNumber: " + invoiceDownload.InvoiceNumber, HttpStatusCode.NotFound, FieldNames.InvoiceNumber, ValidationMessages.NotExist);
            }

            string documentId = list[0].DocumentId;

            return await _documentRepository.GetDocumentAsync(documentId);
        }

        public async Task<ContractFull> GetFullContractAsync(string smContractCode)
        {
            ContractFull response = new ContractFull();
            response.contract = await _contractRepository.GetContractAsync(smContractCode);
            if (response.contract.ContractNumber == null) throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, FieldNames.ContractNumber, ValidationMessages.NotExist);

            decimal price = response.contract.Price > 0 ? response.contract.Price : 0;
            decimal vat = response.contract.Vat != null && response.contract.Vat.Value > 0 ? response.contract.Vat.Value : 0;
            response.contract.TotalPrice = price + vat;

            response.smcontract = await _contractSMRepository.GetAccessCodeAsync(smContractCode);
            response.contract.StoreCode = response.contract.StoreData.StoreCode;
            OpportunityCRM opportunity;
            if (!string.IsNullOrEmpty(response.contract.OpportunityId))
            {
                opportunity = await _opportunityRepository.GetOpportunity(response.contract.OpportunityId);
                response.contract.OpportunityId = opportunity.OpportunityId;
                response.contract.ExpectedMoveIn = opportunity.ExpectedMoveIn;
            }

            PaymentMethodCRM payMetCRM;
            if (!string.IsNullOrEmpty(response.contract.PaymentMethodId))
            {
                payMetCRM = await _paymentMethodRepository.GetPaymentMethodById(response.contract.PaymentMethodId);
                if (payMetCRM != null && !string.IsNullOrWhiteSpace(payMetCRM.Description))
                    response.contract.PaymentMethodDescription = payMetCRM.Description;
            }

            return response;
        }

        public async Task<bool> DocumentExists(string smContractCode)
        {
            DocumentMetadataSearchFilter filter = new DocumentMetadataSearchFilter();
            filter.SmContractCode = smContractCode;
            List<DocumentMetadata> docs = await _documentRepository.Search(filter);

            return docs.Find(x => x.DocumentType == 0) != null;
        }

        public async Task<bool> InvoiceExists(string invoiceNumber)
        {
            DocumentMetadataSearchFilter filter = new DocumentMetadataSearchFilter();
            filter.InvoiceNumber = invoiceNumber;
            List<DocumentMetadata> docs = await _documentRepository.Search(filter);

            return docs.Find(x => x.DocumentType == 3) != null;
        }

        public async Task<string> SaveContractAsync(Document document)
        {
            var savedDocId = string.Empty;

            var metadataInfo = await _documentRepository.SaveDocumentAsync(document);
            if (metadataInfo != null)
            {
                savedDocId = metadataInfo.DocumentId;
                //Contract contract = await _contractRepository.GetContractAsync(document.Metadata.SmContractCode);
                FullContract fullcontract = (await _contractRepository.GetFullContractsBySMCodeAsync(document.Metadata.SmContractCode)).FirstOrDefault();
                var contract = FullContractToContract.Mapper(fullcontract);

                await _contractRepository.UpdateContractAsync(contract);
            }
            return savedDocId;
        }

        public async Task<string> GetContractTimeZoneAsync(string contractNumber)
        {
            var smcontract = await _contractSMRepository.GetAccessCodeAsync(contractNumber);
            return smcontract.Timezone;
        }

        public async Task<UpdateContractsUrlResponse> UpdateContractUrlAsync(int? skip, int? limit)
        {
            UpdateContractsUrlResponse response = new UpdateContractsUrlResponse()
            {
                skip = skip,
                limit = limit
            };

            List<FullContract> contracts = await _contractRepository.GetFullContractsWithoutUrlAsync(limit);
            if (contracts != null && limit >= 1)
            {
                if (skip.HasValue && limit.HasValue)
                    contracts = contracts.Skip(skip.Value).Take(limit.Value).ToList();
                else if (skip.HasValue && !limit.HasValue)
                    contracts = contracts.Skip(skip.Value).ToList();
                else if (!skip.HasValue && limit.HasValue)
                    contracts = contracts.Take(limit.Value).ToList();

                response.NumContracts = contracts.Count();
                _logger.LogInformation($"ContractServices.UpdateContractUrlAsync. Count of contracts to process: {response.NumContracts}.");

                response.ContractsUrl = new List<ContractUrlResponse>();

                foreach (var fullcontract in contracts)
                {
                    ContractUrlResponse contractURLresponse = new ContractUrlResponse();
                    var newContract = FullContractToContract.Mapper(fullcontract);

                    contractURLresponse.ContractNumber = fullcontract.iav_name;
                    contractURLresponse.StoreName = RemoveDiacritics(fullcontract.iav_storeid.StoreName);
                    contractURLresponse.CustomerType = fullcontract.iav_customerid.blue_customertypestring;
                    contractURLresponse.Dni = fullcontract.iav_customerid.iav_dni;
                    contractURLresponse.ContractId = fullcontract.iav_contractid;
                    contractURLresponse.SMContractCode = fullcontract.iav_smcontractcode;
                    contractURLresponse.DocumentRepositoryUrl = fullcontract.iav_storeid.DocumentRepositoryUrl;
                    contractURLresponse.Environment = _configuration["Environment"].ToLower();

                    if (!string.IsNullOrEmpty(newContract.ContractUrl))
                    {
                        contractURLresponse.ContractUrl = newContract.ContractUrl;

                        try
                        {
                            response.ContractsUrl.Add(contractURLresponse);
                            await _contractRepository.UpdateContractAsync(newContract);
                        }
                        catch (Exception ex)
                        {
                            response.Error = $"ContractServices.UpdateContractUrlAsync. Contract: {JsonConvert.SerializeObject(newContract)}";
                            _logger.LogError(ex, $"ContractServices.UpdateContractUrlAsync. Contract: {response.Error}");
                        }
                    }
                }
            }
            return response;
        }

        private string RemoveDiacritics(string text)
        {
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                              UnicodeCategory.NonSpacingMark)
              ).Normalize(NormalizationForm.FormC).Replace(" ", "").ToLower();
        }


    }
}
