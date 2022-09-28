using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Entities.Mappers;
using customerportalapi.Repositories.Interfaces;
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
        private readonly ISignatureRepository _signatureRepository;
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
            ISignatureRepository signatureRepository,
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
            _signatureRepository = signatureRepository;
            _logger = logger;
        }

        public async Task<Contract> GetContractAsync(string contractNumber)
        {
            Contract contract = await _contractRepository.GetContractAsync(contractNumber);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, FieldNames.ContractNumber, ValidationMessages.NotExist);

            return contract;
        }

        public EmailTemplate GetTemplateByLanguage(string language, EmailTemplateTypes template)
        {
            EmailTemplate requestDigitalContractTemplate = null;
            if (!string.IsNullOrEmpty(language))
            {
                requestDigitalContractTemplate = _emailTemplateRepository.getTemplate((int)template, language.ToLower());
            }

            if (requestDigitalContractTemplate == null || string.IsNullOrEmpty(requestDigitalContractTemplate._id))
            {
                requestDigitalContractTemplate = _emailTemplateRepository.getTemplate((int)template, LanguageTypes.en.ToString());
            }

            return requestDigitalContractTemplate;
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

                // Get template Language
                string storeCountryCode = contract?.StoreData?.CountryCode;
                EmailTemplate requestDigitalContractTemplate = GetTemplateByLanguage(storeCountryCode, EmailTemplateTypes.RequestDigitalContract);

                if (string.IsNullOrEmpty(requestDigitalContractTemplate._id))
                {
                    string errorMessage = (int)EmailTemplateTypes.RequestDigitalContract + " : " + EmailTemplateTypes.RequestDigitalContract.ToString() + " : " + storeCountryCode?.ToLower();
                    throw new ServiceException("Email Template not exist, " + errorMessage, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotExist);
                }

                Email message = new Email();
                message.EmailFlow = EmailFlowType.DownloadContract.ToString();
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

                // Get template Language
                string storeCountryCode = store?.CountryCode;
                EmailTemplate requestDigitalInvoiceTemplate = GetTemplateByLanguage(storeCountryCode, EmailTemplateTypes.RequestDigitalInvoice);

                if (string.IsNullOrEmpty(requestDigitalInvoiceTemplate._id))
                {
                    string errorMessage = (int)EmailTemplateTypes.RequestDigitalInvoice + " : " + EmailTemplateTypes.RequestDigitalInvoice.ToString() + " : " + storeCountryCode?.ToLower();
                    throw new ServiceException("Email Template not exist, " + errorMessage, HttpStatusCode.NotFound, FieldNames.Email + FieldNames.Template, ValidationMessages.NotExist);
                }

                Email message = new Email();
                message.EmailFlow = EmailFlowType.DownloadInvoice.ToString();
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
                if (payMetCRM != null)
                {
                    response.contract.PaymentMethodClass = new PaymentMethod()
                    {
                        BankAccountPayment = payMetCRM.BankAccountPayment,
                        CardPayment = payMetCRM.CardPayment
                    };
                    if (!string.IsNullOrWhiteSpace(payMetCRM.Description))
                    {
                        response.contract.PaymentMethodDescription = payMetCRM.Description;
                    }
                }
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

        public async Task<SignatureResultDataResponse> UpdateContractsWithoutSignatureId(string fromCreatedOn, string toCreatedOn = null, string arrContracts = null, string status = null)
        {
            var result = new SignatureResultDataResponse();

            //var listFullContract = await _contractRepository.GetFullContractsBySMCodeAsync("RI227MI204901C0N8000");

            // Obtener contratos de CRM
            var listFullContract = new List<FullContract>() { };
            if (!string.IsNullOrEmpty(arrContracts))
            {
                var listContractIds = arrContracts.Replace("\"", "").Replace(" ", "").Split(",");

                foreach (var contractId in listContractIds)
                {
                    try
                    {
                        var fullContract = await _contractRepository.GetFullContractsByCRMCodeAsync(contractId);
                        listFullContract.Add(fullContract);
                    }
                    catch
                    {

                    }

                }
            }
            else
            {
                listFullContract = await _contractRepository.GetFullContractsWithoutSignaturitId(fromCreatedOn, toCreatedOn);
            }


            if (listFullContract != null)
            {
                // por cada contrato, consultar en signaturit
                foreach (var fullcontract in listFullContract)
                {
                    try
                    {
                        var signaturitContracts = await _signatureRepository.GetSignatureInfoAsync(fullcontract.iav_name, fromCreatedOn, fullcontract.iav_storeid.CountryCode, status);

                        var contract = FullContractToContract.Mapper(fullcontract);
                        result.ListContracts.Add(contract);
                        if (signaturitContracts != null && signaturitContracts.Any() &&
                            signaturitContracts.FirstOrDefault()?.Documents != null &&
                            signaturitContracts.FirstOrDefault().Documents.Any())
                        {
                            var signContract = signaturitContracts.FirstOrDefault();
                            result.ListSignatureResultData.Add(signContract);
                            try
                            {
                                var resultDB = await _signatureRepository.Create(signContract);
                            }
                            catch
                            {
                            }

                            switch (signContract.Documents.FirstOrDefault().Status.ToLower())
                            {
                                case "completed":
                                    contract.SignatureStatus = "audit_trail_completed";
                                    break;
                                case "canceled":
                                    contract.SignatureStatus = "document_canceled";
                                    contract.ContractUrl = "";
                                    break;
                                case "expired":
                                    contract.SignatureStatus = "document_expired";
                                    contract.ContractUrl = "";
                                    break;
                                default:
                                    contract.SignatureStatus = signContract.Documents.FirstOrDefault().Status;
                                    contract.ContractUrl = "";
                                    break;
                            }

                            contract.SignatureIdSignature = signContract.Id.ToString();
                            contract.DocumentIdSignature = signContract.Documents.FirstOrDefault().Id.ToString();

                            var updatedContract = await _contractRepository.UpdateContractAsync(contract);
                        }
                    }
                    catch
                    {
                        result.ListContractsNoProcessed.Add(fullcontract.iav_name);
                    }
                }
            }

            return result;
        }

        public async Task<List<KeyValuePair<string, string>>> UploadDocuments(string arrContracts, string status = null)
        {

            var listContractsUploaded = new List<KeyValuePair<string, string>>();


            // Obtener contratos de CRM
            var listFullContract = new List<FullContract>() { };
            if (!string.IsNullOrEmpty(arrContracts))
            {
                var listContractIds = arrContracts.Replace("\"", "").Replace(" ", "").Split(",");

                foreach (var contractId in listContractIds)
                {

                    try
                    {
                        var fullContract = await _contractRepository.GetFullContractsByCRMCodeAsync(contractId);
                        listFullContract.Add(fullContract);

                        listContractsUploaded.Add(new KeyValuePair<string, string>(contractId, string.Empty));
                    }
                    catch
                    {
                        listContractsUploaded.Add(new KeyValuePair<string, string>(contractId, "Sin contrato en CRM"));
                    }
                }
            }

            if (listFullContract.Any())
            {
                // por cada contrato
                foreach (var fullcontract in listFullContract)
                {
                    var docMetadata = new DocumentMetadata()
                    {
                        AccountDni = fullcontract.iav_customerid.iav_dni,
                        AccountType =
                            UserInvitationUtils.GetUserType(fullcontract.iav_customerid.blue_customertypestring),
                        ContractNumber = fullcontract.iav_name,
                        SmContractCode = fullcontract.iav_smcontractcode,
                        BankAccountOrderNumber = string.Empty,
                        BankAccountName = string.Empty,
                        CreatedBy = string.Empty,
                        DocumentType = 0,
                        StoreName = fullcontract.iav_storeid.StoreName
                    };

                    var docId = string.Empty;
                    try
                    {
                        var strSince = fullcontract.iav_contractdate.ToString("yyyy-MM-dd");
                        docId = await _signatureRepository.UploadDocumentAsync(docMetadata, fullcontract.iav_storeid.CountryCode, strSince, status);
                    }
                    catch
                    {

                    }

                    var removalStatus = listContractsUploaded.RemoveAll(x => x.Key == fullcontract.iav_name);
                    if (removalStatus == 1)
                    {
                        listContractsUploaded.Add(new KeyValuePair<string, string>(fullcontract.iav_name, docId));
                    }
                }
            }

            return listContractsUploaded;
        }

        public async Task<ListContractStatusResponseList> UpdateContractStatusInCrm(List<ContractStatusRequest> contactListIds)
        {
            var result = new ListContractStatusResponseList();

            var lstContractStatusResponse = new List<ContractStatusResponse>() { };

            var ListSignatureResultData = new List<SignatureResultData>() { };
            var ListContracts = new List<Entities.Contract>() { };

            var listContractsNoProcessed = new List<ListContractsNoProcessed> { };

            // por cada id de signaturit
            foreach (var requestContractId in contactListIds)
            {
                var contractStatusResponse = new ContractStatusResponse() { };

                try
                {
                    var signaturitId = requestContractId.SignatureId;
                    var signaturitStatusExcel = requestContractId.Status;

                    contractStatusResponse.SignatureId = signaturitId;
                    contractStatusResponse.ExcelStatus = signaturitStatusExcel;

                    // se comprueba si ya se ha procesado este ID
                    if (string.IsNullOrEmpty(signaturitId) ||
                        ListSignatureResultData?.Count(c => c.Id.ToString() == signaturitId) > 0 ||
                        listContractsNoProcessed?.Count(c => c.SignatureId == signaturitId) > 0)
                        continue;

                    var noProcessed = new ListContractsNoProcessed() { };
                    if (listContractsNoProcessed.Count(c => c.SignatureId == signaturitId) <= 0)
                        noProcessed.SignatureId = signaturitId;


                    // se obtiene el contrato de signaturit
                    var signaturitContract = await _signatureRepository.GetSignatureInfoByIdAsync(requestContractId.SignatureId);
                    ListSignatureResultData.Add(signaturitContract);

                    if (signaturitContract?.Documents != null && signaturitContract.Data.Any())
                    {
                        signaturitContract.Data.TryGetValue("00_contract_ContractNumber", out var iav_name);
                        var signatureStatus = signaturitContract?.Documents.FirstOrDefault().Status;

                        contractStatusResponse.StatusSignaturitNow = signatureStatus;

                        if (string.IsNullOrEmpty(iav_name))
                            continue;

                        noProcessed.CrmContractId = iav_name;

                        if (!string.IsNullOrEmpty(noProcessed.SignatureId))
                            listContractsNoProcessed.Add(noProcessed);

                        // se obtiene el contrato de crm
                        var fullContract = await _contractRepository.GetFullContractsByCRMCodeAsync(iav_name);
                        var contract = FullContractToContract.Mapper(fullContract); // el mapper rellena el campo ContractUrl
                        ListContracts.Add(contract);

                        contractStatusResponse.CrmContractId = iav_name;
                        contractStatusResponse.CrmOldStatus = contract.SignatureStatus;
                        contractStatusResponse.CrmOldContractUrl = fullContract.new_contacturl;
                        contractStatusResponse.CrmOldSignatureIdSignature = contract.SignatureIdSignature;
                        contractStatusResponse.CrmOldDocumentIdSignature = contract.DocumentIdSignature;

                        string newSignatureStatus;
                        switch (signatureStatus.ToLower())
                        {
                            case "completed":
                                newSignatureStatus = "audit_trail_completed";
                                contractStatusResponse.CrmNewContractUrl = contract.ContractUrl;
                                break;
                            case "canceled":
                                newSignatureStatus = "document_canceled";
                                contract.ContractUrl = contractStatusResponse.CrmNewContractUrl = "";
                                break;
                            case "expired":
                                newSignatureStatus = "document_expired";
                                contract.ContractUrl = contractStatusResponse.CrmNewContractUrl = "";
                                break;
                            default:
                                newSignatureStatus = signatureStatus;
                                contract.ContractUrl = contractStatusResponse.CrmNewContractUrl = "";
                                break;
                        }

                        //contractStatusResponse.CrmContractUrl = newContractUrl;
                        contractStatusResponse.CrmNewStatus = newSignatureStatus;
                        contractStatusResponse.CrmNewSignatureIdSignature = signaturitContract.Id.ToString();
                        contractStatusResponse.CrmNewDocumentIdSignature = signaturitContract.Documents.FirstOrDefault().Id.ToString();

                        if (contractStatusResponse.CrmOldStatus != "audit_trail_completed"
                            && contractStatusResponse.CrmOldStatus != contractStatusResponse.CrmNewStatus)
                        {
                            contract.SignatureStatus = contractStatusResponse.CrmNewStatus;
                            contract.SignatureIdSignature = contractStatusResponse.CrmNewSignatureIdSignature;
                            contract.DocumentIdSignature = contractStatusResponse.CrmNewDocumentIdSignature;

                            var updatedContract = await _contractRepository.UpdateContractAsync(contract);

                            contractStatusResponse.CrmUpdated = true;
                        }

                        try
                        {
                            listContractsNoProcessed.Remove(noProcessed);
                        }
                        catch { }

                    }
                }
                catch { }

                lstContractStatusResponse.Add(contractStatusResponse);
            }

            result.ListContractStatusResponse = lstContractStatusResponse;
            result.ListContractsNoProcessed = listContractsNoProcessed;
            result.ListSignatureResultData = ListSignatureResultData;
            result.ListContracts = ListContracts;

            return result;
        }
    }
}