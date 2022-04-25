using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using customerportalapi.Entities.Mappers;

namespace customerportalapi.Services
{
    public class PaymentServices : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IProcessRepository _processRepository;
        private readonly ISignatureRepository _signatureRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IAccountSMRepository _accountSMRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IContractSMRepository _contractSMRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IPayRepository _payRepository;
        private readonly ILogger<PaymentServices> _logger;

        public PaymentServices(
            IConfiguration configuration,
            IUserRepository userRepository,
            IProcessRepository processRepository,
            ISignatureRepository signatureRepository,
            IStoreRepository storeRepository,
            IAccountSMRepository accountSMRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IMailRepository mailRepository,
            IProfileRepository profileRepository,
            IContractRepository contractRepository,
            IPaymentRepository paymentRepository,
            IContractSMRepository contractSMRepository,
            ICardRepository cardRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPayRepository payRepository,
            ILogger<PaymentServices> logger
        )
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _signatureRepository = signatureRepository;
            _storeRepository = storeRepository;
            _profileRepository = profileRepository;
            _accountSMRepository = accountSMRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _mailRepository = mailRepository;
            _profileRepository = profileRepository;
            _contractRepository = contractRepository;
            _paymentRepository = paymentRepository;
            _contractSMRepository = contractSMRepository;
            _cardRepository = cardRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _payRepository = payRepository;
            _logger = logger;
        }

        public async Task<bool> ChangePaymentMethod(PaymentMethod paymentMethod)
        {
            //1. User must exists
            int userType = UserInvitationUtils.GetUserType(paymentMethod.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(paymentMethod.Dni, userType);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //2. Process paymentmethod change
            PaymentMethodBank bankmethod = (PaymentMethodBank)paymentMethod;

            //3. Validate contract number
            if (string.IsNullOrEmpty(bankmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "ContractNumber", "Empty fields");

            //4. Validate that don't exist any pending process for same user, process type and SM contract code
            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.UserName = user.Username;
            searchProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeBank;
            searchProcess.SmContractCode = bankmethod.SmContractCode;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Pending;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 0)
                throw new ServiceException("User have same pending process for this contract number", HttpStatusCode.BadRequest, "ContractNumber", "Pending process");

            //5. Get Aps
            ApsRequest request = new ApsRequest()
            {
                Dni = user.Dni,
                Username = user.Username,
                ContractNumber = bankmethod.SmContractCode,
                IBAN = bankmethod.IBAN
            };

            ApsData aps = await _contractSMRepository.UpdateAps(request);
            bankmethod.ApsReference = aps.Reference;

            //6. Generate and send Document To SignatureAPI
            var store = await _storeRepository.GetStoreAsync(bankmethod.StoreCode);

            var form = FillFormBankMethod(store, bankmethod, user);

            _logger.LogInformation($"PaymentServices.ChangePaymentMethod().CreateSignature. form: {JsonConvert.SerializeObject(form)}");

            Guid documentid = await _signatureRepository.CreateSignature(form);

            _logger.LogInformation($"PaymentServices.ChangePaymentMethod().CreateSignature. documentid: {documentid}");

            //7. Get & update account
            AccountProfile account = await _profileRepository.GetAccountByDocumentNumberAsync(user.Dni);
            account.TokenUpdate = ((int)TokenUpdateTypes.Pending).ToString();
            account.TokenUpdateDate = account.TpvSincronizationDate = DateTime.Now.ToString("O");
            // Como aun no se ha firmado el documento SEPA, no se puede grabar el IBAN en CRM
            account = await _profileRepository.UpdateAccountAsync(account);

            //8. Create a change method payment process
            Process process = new Process();
            process.Username = user.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeBank;
            process.ProcessStatus = (int)ProcessStatuses.Pending;
            process.ContractNumber = bankmethod.ContractNumber;
            process.SmContractCode = bankmethod.SmContractCode;
            process.Documents.Add(new ProcessDocument()
            {
                DocumentId = documentid.ToString(),
                DocumentStatus = "document_pending",
            });

            await _processRepository.Create(process);


            return true;
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

        public async Task<bool> UpdatePaymentBankProcess(SignatureStatus value)
        {
            _logger.LogInformation($"PaymentServices.UpdatePaymentProcess(SignatureStatus). value:{JsonConvert.SerializeObject(value)}.");

            // 1.- Get User info from Mongo
            User user = _userRepository.GetCurrentUserByUsername(value.User);
            string usertype = user.Usertype == (int)UserTypes.Business ? AccountType.Business : string.Empty;

            // 2.- Get info of account by DNI for update the payments data
            AccountProfile account = await _profileRepository.GetAccountAsync(user.Dni, usertype);

            // 3.- Get signatureprocess data from signaturitAPI
            SignatureSearchFilter filter = new SignatureSearchFilter();
            filter.Filters.SignatureId = value.SignatureId;
            List<SignatureProcess> signatures = await _signatureRepository.SearchSignaturesAsync(filter);
            if (signatures.Count != 1)
                throw new ServiceException($"PaymentServices.UpdatePaymentProcess(). Error searching signature for this process. SignatureId:{value.SignatureId}.", HttpStatusCode.BadRequest, "SignatureId", "Not exist");

            ProcessedDocument processedpaymentdocument = null;
            var docIndex = 0;
            foreach (SignatureDocumentResult dr in signatures[0].SignatureResult.Documents)
            {
                if (dr.Id.ToString() == value.DocumentId)
                    processedpaymentdocument = signatures[0].Documents[docIndex];
            }

            string smContractCode = processedpaymentdocument.SmContractCode;

            // 5.- Get the FullContract with store data
            FullContract fullcontract = (await _contractRepository.GetFullContractsBySMCodeAsync(smContractCode)).FirstOrDefault();
            var contract = FullContractToContract.Mapper(fullcontract);

            // 4.- Get the template "UpdatePaymentMethod"
            EmailTemplate template = GetTemplateByLanguage(contract?.StoreData?.CountryCode, EmailTemplateTypes.UpdatePaymentMethod);

            if (contract == null || contract.StoreData == null || contract.StoreData.StoreId == null)
                throw new ServiceException($"PaymentServices.UpdatePaymentProcess(). Store not found. StoreCode:{contract?.StoreData?.StoreCode}. StoreId.", HttpStatusCode.BadRequest, "StoreId");

            // 6.- Get the PaymentMethods
            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethodByBankAccount(contract.StoreData.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException($"PaymentServices.UpdatePaymentProcess(). Error get payment method crm. StoreName:{contract.StoreData.StoreName}, StoreId:{contract.StoreData.StoreId}. SMId.", HttpStatusCode.BadRequest, "SMId");

            var updateAcc = false;
            // 7.- Get info of bankaccount from Mongo db spacemanager.apsreferences
            if (!string.IsNullOrEmpty(processedpaymentdocument.BankAccountOrderNumber.Trim()) && processedpaymentdocument.BankAccountOrderNumber != "null")
            {
                account.BankAccount = processedpaymentdocument.BankAccountOrderNumber;
                updateAcc = true;
            }
            else
            {
                // Get IBAN from Aps reference table
                var aps = (await _contractSMRepository.GetApssByField("username", user.Username)).Where(x => !string.IsNullOrEmpty(x.IBAN)).LastOrDefault();
                if (aps != null && !string.IsNullOrEmpty(aps.IBAN))
                {
                    account.BankAccount = aps.IBAN;
                    updateAcc = true;
                }
                else
                {
                    throw new ServiceException($"PaymentServices.UpdatePaymentProcess(). No IBAN found in Aps. Username:{user.Username}", HttpStatusCode.BadRequest);
                }
            }

            if (updateAcc)
            {
                account.PaymentMethodId = payMetCRM.PaymentMethodId;
                account.TokenUpdate = ((int)TokenUpdateTypes.OK).ToString();
                account.TpvSincronizationDate = DateTime.Now.ToString("O");

                // 7.1.- Update account CRM
                AccountProfile updatedAccount = await _profileRepository.UpdateAccountAsync(account);
                if (updatedAccount.SmCustomerId == null)
                    throw new ServiceException("PaymentServices.UpdatePaymentProcess(). Error updating account. SmCustomerId.", HttpStatusCode.BadRequest, "SmCustomerId");
            }

            // 8.- Update contract CRM
            contract.PaymentMethodId = payMetCRM.PaymentMethodId;
            var paymentName = !String.IsNullOrEmpty(payMetCRM.Description) ? payMetCRM.Description : payMetCRM.Name;

            Contract updateContract = await _contractRepository.UpdateContractAsync(contract);
            if (updateContract.ContractNumber == null)
                throw new ServiceException("PaymentServices.UpdatePaymentProcess(). Error updating contract. ContractNumber.", HttpStatusCode.BadRequest, "ContractNumber");

            _logger.LogInformation($"PaymentServices.UpdatePaymentProcess(). Template StoreMail Information id:{template._id}.");
            if (template._id != null)
            {
                // 9.- Send Mail
                Email message = new Email();
                message.EmailFlow = EmailFlowType.UpdatePayment.ToString();
                string storeMail = contract.StoreData.EmailAddress1;
                _logger.LogInformation("PaymentServices.UpdatePaymentProcess(). Entering StoreMail Information", storeMail);
                if (storeMail == null) throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);
                if (!(_configuration["Environment"] == nameof(EnvironmentTypes.PRO))) storeMail = _configuration["MailStores"];

                message.To.Add(storeMail);
                _logger.LogInformation("PaymentServices.UpdatePaymentProcess(). StoreMail Information", storeMail);
                message.Subject = string.Format(template.subject, user.Name, user.Dni);
                message.Body = string.Format(template.body, user.Name, user.Dni, processedpaymentdocument.DocumentNumber, paymentName);
                _logger.LogInformation("PaymentServices.UpdatePaymentProcess(). Sending StoreMail Information", storeMail);
                await _mailRepository.Send(message);
            }
            return true;
        }

        private MultipartFormDataContent FillFormBankMethod(Store store, PaymentMethodBank bankmethod, User user)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(user.Name), "recipients[0][name]");
            form.Add(new StringContent(user.Email), "recipients[0][email]");
            form.Add(new StringContent(((int)DocumentTypes.SEPA).ToString()), "documentinformation[0][documenttype]");
            form.Add(new StringContent(bankmethod.ContractNumber), "documentinformation[0][documentidentificationnumber]");
            form.Add(new StringContent(bankmethod.IBAN), "documentinformation[0][bankaccountordernumber]");
            form.Add(new StringContent(store.CompanyName), "documentinformation[0][bankaccountname]");
            form.Add(new StringContent(bankmethod.SmContractCode), "documentinformation[0][smcontractcode]");
            form.Add(new StringContent(store.CountryCode), "documentinformation[0][documentcountry]");
            form.Add(new StringContent(String.IsNullOrEmpty(user.Language) ? store.CountryCode : user.Language.ToUpper()), "documentinformation[0][documentlanguage]");
            form.Add(new StringContent(store.StoreName), "storeidentification");
            form.Add(new StringContent(SystemTypes.CustomerPortal.ToString()), "sourcesystem");
            form.Add(new StringContent(user.Username), "sourceuser");
            form.Add(new StringContent(user.Usertype.ToString()), "accounttype");
            form.Add(new StringContent(user.Dni.ToString()), "accountdni");
            form.Add(new StringContent(_configuration["GatewaySignatureEventsUrl"]), "signatureendprocess_url");

            // data
            form.Add(new StringContent("contractnumber"), "data[0][key]");
            form.Add(new StringContent("companycountry"), "data[1][key]");
            form.Add(new StringContent("companylegalname"), "data[2][key]");
            form.Add(new StringContent("companycif"), "data[3][key]");
            form.Add(new StringContent("companylegaladdress"), "data[4][key]");
            form.Add(new StringContent("clientfullname"), "data[5][key]");
            form.Add(new StringContent("iban"), "data[6][key]");
            form.Add(new StringContent("clientaddress"), "data[7][key]");
            form.Add(new StringContent("clientpostalcode"), "data[8][key]");
            form.Add(new StringContent("clientcountry"), "data[9][key]");
            form.Add(new StringContent("storecity"), "data[10][key]");
            //form.Add(new StringContent("date"), "data[11][key]");
            form.Add(new StringContent("aps"), "data[11][key]");
            form.Add(new StringContent(bankmethod.ContractNumber), "data[0][value]");
            form.Add(new StringContent(store.Country), "data[1][value]");
            form.Add(new StringContent(store.CompanyName), "data[2][value]");
            form.Add(new StringContent(store.CompanyCif), "data[3][value]");
            form.Add(new StringContent(store.CompanySocialAddress), "data[4][value]");
            form.Add(new StringContent(bankmethod.FullName), "data[5][value]");
            form.Add(new StringContent(bankmethod.IBAN), "data[6][value]");
            form.Add(new StringContent(bankmethod.Address), "data[7][value]");
            form.Add(new StringContent(bankmethod.PostalCode), "data[8][value]");
            form.Add(new StringContent(bankmethod.Country), "data[9][value]");
            form.Add(new StringContent(store.City), "data[10][value]");
            //form.Add(new StringContent(DateTime.Today(short)), "data[11][value]");
            form.Add(new StringContent(bankmethod.ApsReference), "data[11][value]");

            _logger.LogInformation($"PaymentServices.FillFormBankMethod(). form: {JsonConvert.SerializeObject(form)}");

            return form;

        }

        public async Task<string> ChangePaymentMethodCardLoad(PaymentMethodCard paymentMethod)
        {
            //1. User must exists
            int userType = UserInvitationUtils.GetUserType(paymentMethod.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(paymentMethod.Dni, userType);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Dni, ValidationMessages.NotExist);

            // 2. Validate data
            PaymentMethodCard cardmethod = (PaymentMethodCard)paymentMethod;

            // 3. Get data to load card form string

            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, paymentMethod.AccountType);

            if (string.IsNullOrEmpty(cardmethod.IdCustomer))
            {
                SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(cardmethod.SmContractCode);
                if (!string.IsNullOrEmpty(smContract.Customerid))
                    cardmethod.IdCustomer = smContract.Customerid;
            }

            cardmethod.ExternalId = Guid.NewGuid().ToString();

            checkFieldsCardLoad(cardmethod);

            ProcessSearchFilter filter = new ProcessSearchFilter()
            {
                UserName = user.Username,
                SmContractCode = cardmethod.SmContractCode,
                ProcessType = (int)ProcessTypes.PaymentMethodChangeCard,
                ProcessStatus = (int)ProcessStatuses.Started
            };
            bool result = await CancelProcessByFilter(filter);

            HttpContent content = FillFormUrlEncodedCardMethod(userProfile, cardmethod, user);

            string stringHtml = await _paymentRepository.ChangePaymentMethodCard(content);
            Card card = new Card()
            {
                SmContractCode = cardmethod.SmContractCode,
                ContractNumber = cardmethod.ContractNumber,
                ExternalId = cardmethod.ExternalId,
                Username = user.Username
            };
            bool createCard = await _cardRepository.Create(card);
            if (createCard == false)
                throw new ServiceException("Error creating card", HttpStatusCode.BadRequest);

            Process process = new Process();
            process.Username = user.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
            process.ProcessStatus = (int)ProcessStatuses.Started;
            process.ContractNumber = cardmethod.ContractNumber;
            process.SmContractCode = cardmethod.SmContractCode;
            process.Card = new ProcessCard()
            {
                ExternalId = cardmethod.ExternalId,
                Status = 0,
                Email = cardmethod.Email,
                PhoneNumber = cardmethod.PhoneNumber,
                Address = cardmethod.Address
            };
            process.Documents = null;

            await _processRepository.Create(process);

            return stringHtml;
        }

        private HttpContent FillFormUrlEncodedCardMethod(Profile userProfile, PaymentMethodCard cardmethod, User user)
        {
            string language = getCountryLangByLanguage(user.Language);
            string phoneNumber = cardmethod.PhonePrefix + "|" + cardmethod.PhoneNumber;
            phoneNumber = phoneNumber.Replace(" ", "");
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("externalid", cardmethod.ExternalId));
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("siteid", cardmethod.SiteId));
            keyValues.Add(new KeyValuePair<string, string>("name", userProfile.Name));
            keyValues.Add(new KeyValuePair<string, string>("surnames", userProfile.Surname));
            keyValues.Add(new KeyValuePair<string, string>("nif", userProfile.DocumentNumber));
            keyValues.Add(new KeyValuePair<string, string>("idcustomer", cardmethod.IdCustomer));
            keyValues.Add(new KeyValuePair<string, string>("url", _configuration["ChangePaymentMethodCardResponse"]));
            keyValues.Add(new KeyValuePair<string, string>("language", language));
            keyValues.Add(new KeyValuePair<string, string>("HPP_CUSTOMER_EMAIL", cardmethod.Email));
            keyValues.Add(new KeyValuePair<string, string>("HPP_CUSTOMER_PHONENUMBER_MOBILE", phoneNumber));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET1", cardmethod.Address.Street1));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET2", cardmethod.Address.Street2));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_STREET3", cardmethod.Address.Street2));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_CITY", cardmethod.Address.City));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_POSTALCODE", cardmethod.Address.ZipOrPostalCode));
            keyValues.Add(new KeyValuePair<string, string>("HPP_BILLING_COUNTRY", cardmethod.CountryISOCodeNumeric));

            _logger.LogInformation($"PaymentServices.FillFormUrlEncodedCardMethod(). keyValues: {JsonConvert.SerializeObject(keyValues)}");

            HttpContent content = new FormUrlEncodedContent(keyValues);
            return content;
        }

        private string getCountryLangByLanguage(string language)
        {
            switch (language)
            {
                case "en":
                    return "en_GB";
                case "es":
                    return "es_ES";
                case "ca":
                    return "ca_ES";
                case "gl":
                    return "gl_ES";
                case "eu":
                    return "eu_ES";
                case "pt":
                    return "pt_PT";
                case "fr":
                    return "fr_FR";
                case "de":
                    return "de_DE";
                case "da":
                    return "da_DK";
                case "nl":
                    return "nl_NL";
                case "el":
                    return "el_GR";
                case "hu":
                    return "hu_HU";
                case "is":
                    return "is_IS";
                case "it":
                    return "it_IT";
                case "bg":
                    return "bg_BG";
                case "cs":
                    return "cs_CZ";
                case "lv":
                    return "lv_LV";
                case "lt":
                    return "lt_LT";
                case "no":
                    return "no_NO";
                case "pl":
                    return "pl_PL";
                case "ro":
                    return "ro_RO";
                case "et":
                    return "et_EE";
                case "fi":
                    return "fi_FI";
                case "tr":
                    return "tr_TR";
                default:
                    return "en_GB";
            }
        }

        public async Task<bool> ChangePaymentMethodCardResponseAsync(PaymentMethodCardData cardData)
        {

            string data = JsonConvert.SerializeObject(cardData);
            _logger.LogInformation("ChangePaymentMethodCardResponseAsync:" + data);

            // 0. Guardar paymentMethodData en colección Cards
            Card findCard = _cardRepository.GetByExternalId(cardData.ExternalId);

            if (findCard.Id == null)
            {
                PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
                {
                    ExternalId = cardData.ExternalId,
                    Confirmed = false,
                    Channel = "WEBPORTAL"
                };

                await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);

                throw new ServiceException("Card doesn´t exist", HttpStatusCode.BadRequest);
            }

            Card card = new Card()
            {
                Id = findCard.Id,
                ExternalId = cardData.ExternalId,
                Idcustomer = cardData.idCustomer,
                Siteid = cardData.SiteId,
                Token = cardData.Token,
                Status = cardData.Status,
                Message = cardData.Message,
                Cardholder = cardData.CardHolder,
                Expirydate = cardData.expiryDate,
                Typecard = cardData.typeCard,
                Cardnumber = cardData.cardNumber,
                ContractNumber = findCard.ContractNumber,
                SmContractCode = findCard.SmContractCode,
                Username = findCard.Username,
                Current = false

            };
            Card updateCard = _cardRepository.Update(card);

            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.CardExternalId = updateCard.ExternalId;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more started process for this externalId", HttpStatusCode.BadRequest, "CardExternalId", "Pending process");


            // Card verification failed
            if (cardData.Status != "00")
            {
                Process cancelProcess = new Process();
                cancelProcess.Id = processes[0].Id;
                cancelProcess.Username = updateCard.Username;
                cancelProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
                cancelProcess.ProcessStatus = (int)ProcessStatuses.Canceled;
                cancelProcess.ContractNumber = updateCard.ContractNumber;
                cancelProcess.SmContractCode = updateCard.SmContractCode;
                cancelProcess.Card = processes[0].Card;
                cancelProcess.Card.ExternalId = updateCard.ExternalId;
                cancelProcess.Card.Status = 0;
                cancelProcess.Documents = null;

                _processRepository.Update(cancelProcess);
                throw new ServiceException("Error card verification", HttpStatusCode.BadRequest);
            }

            if (processes.Count == 1 && processes[0].ProcessStatus == (int)ProcessStatuses.Started)
            {

                PaymentMethodCardSignature paymentMethodCardSignature = await GetPaymentMethodCardSignature(processes[0], card);
                bool result = await ChangePaymentMethodCard(paymentMethodCardSignature);

                return result;
            }

            return false;
        }
        public async Task<bool> ChangePaymentMethodCard(PaymentMethodCardSignature paymentMethodCardSignature)
        {
            string payment = JsonConvert.SerializeObject(paymentMethodCardSignature);
            _logger.LogInformation("ChangePaymentMethodCard:" + payment);
            //1. User must exists
            PaymentMethodCardSignature cardmethod = (PaymentMethodCardSignature)paymentMethodCardSignature;
            User user = _userRepository.GetCurrentUserByUsername(cardmethod.Username);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.NotExist);

            //2. Process paymentmethod change card

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "ContractNumber", "Empty fields");


            var store = await _storeRepository.GetStoreAsync(cardmethod.SiteId);

            Card card = _cardRepository.GetByExternalId(cardmethod.ExternalId);
            if (card.Id == null)
            {
                PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
                {
                    ExternalId = cardmethod.ExternalId,
                    Confirmed = false,
                    Channel = "WEBPORTAL"
                };

                PaymentMethodCardConfirmationResponse cardConfirmation = await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
                throw new ServiceException("Card not found", HttpStatusCode.BadRequest, "CardExternalId");
            }

            cardmethod.CardHolderName = card.Cardholder;
            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, cardmethod.AccountType);

            // 2. Update process confirmCardSignature
            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.UserName = user.Username;
            searchProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
            searchProcess.SmContractCode = cardmethod.SmContractCode;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            searchProcess.CardExternalId = cardmethod.ExternalId;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more started process for this externalId & ProcessType", HttpStatusCode.BadRequest, "CardExternalId", "Started process");

            if (processes.Count == 0)
                throw new ServiceException("User don't have started process for this externalId & ProcessType", HttpStatusCode.BadRequest, "CardExternalId", "Started process");

            // if current process if for change to Card, use card email for user notification
            if (!string.IsNullOrEmpty(processes[0].Id) && !string.IsNullOrEmpty(processes[0].Card.Email))
            {
                user.Email = processes[0].Card.Email;
            }

            var form = FillFormCardMethod(store, cardmethod, user, userProfile);

            _logger.LogInformation($"PaymentServices.ChangePaymentMethod().CreateSignature. form: {JsonConvert.SerializeObject(form)}");

            Guid documentid = await _signatureRepository.CreateSignature(form);

            _logger.LogInformation($"PaymentServices.ChangePaymentMethod().CreateSignature. documentid: {documentid}");

            Process process = new Process();
            process.Id = processes[0].Id;
            process.Username = user.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeCardSignature;
            process.ProcessStatus = (int)ProcessStatuses.Pending;
            process.ContractNumber = cardmethod.ContractNumber;
            process.SmContractCode = cardmethod.SmContractCode;
            process.CreationDate = processes[0].CreationDate;
            process.ModifiedDate = System.DateTime.Now;
            process.Card = processes[0].Card;
            process.Card.ExternalId = cardmethod.ExternalId;
            process.Card.Status = 1;
            process.Card.Update = processes[0].Card.Update;
            process.Documents.Add(new ProcessDocument()
            {
                DocumentId = documentid.ToString(),
                DocumentStatus = "document_pending",
            });

            _processRepository.Update(process);

            // 3. Update Card
            card.DocumentId = documentid.ToString();
            _cardRepository.Update(card);

            return true;
        }
        private MultipartFormDataContent FillFormCardMethod(Store store, PaymentMethodCardSignature cardmethod, User user, Profile userProfile)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(user.Name), "recipients[0][name]");
            form.Add(new StringContent(user.Email), "recipients[0][email]");
            form.Add(new StringContent(((int)DocumentTypes.Card).ToString()), "documentinformation[0][documenttype]");
            form.Add(new StringContent(cardmethod.ContractNumber), "documentinformation[0][documentidentificationnumber]");
            form.Add(new StringContent(store.CompanyName), "documentinformation[0][bankaccountname]");
            form.Add(new StringContent(cardmethod.SmContractCode), "documentinformation[0][smcontractcode]");
            form.Add(new StringContent(store.CountryCode), "documentinformation[0][documentcountry]");
            form.Add(new StringContent(String.IsNullOrEmpty(user.Language) ? store.CountryCode : user.Language.ToUpper()), "documentinformation[0][documentlanguage]");
            form.Add(new StringContent(store.StoreName), "storeidentification");
            form.Add(new StringContent(SystemTypes.CustomerPortal.ToString()), "sourcesystem");
            form.Add(new StringContent(user.Username), "sourceuser");
            form.Add(new StringContent(user.Usertype.ToString()), "accounttype");
            form.Add(new StringContent(user.Dni.ToString()), "accountdni");
            form.Add(new StringContent(_configuration["GatewaySignatureEventsUrl"]), "signatureendprocess_url");

            // data
            form.Add(new StringContent("customername"), "data[0][key]");
            form.Add(new StringContent("contractnumber"), "data[1][key]");
            form.Add(new StringContent("unitnumber"), "data[2][key]");
            form.Add(new StringContent("cardholdername"), "data[3][key]");
            form.Add(new StringContent("cardholdercif"), "data[4][key]");
            form.Add(new StringContent("cardhoderaddress"), "data[5][key]");
            form.Add(new StringContent("cardhodercp"), "data[6][key]");
            form.Add(new StringContent("cardhodercity"), "data[7][key]");
            form.Add(new StringContent("companylegalname"), "data[8][key]");
            form.Add(new StringContent("companycif"), "data[9][key]");
            form.Add(new StringContent("storecity"), "data[10][key]");
            form.Add(new StringContent(userProfile.Fullname), "data[0][value]");
            form.Add(new StringContent(cardmethod.ContractNumber), "data[1][value]");
            form.Add(new StringContent(cardmethod.UnitNumber), "data[2][value]");
            form.Add(new StringContent(cardmethod.CardHolderName), "data[3][value]");
            form.Add(new StringContent(cardmethod.CardHolderCif), "data[4][value]");
            form.Add(new StringContent(cardmethod.CardHolderAddress), "data[5][value]");
            form.Add(new StringContent(cardmethod.CardHolderPostalCode), "data[6][value]");
            form.Add(new StringContent(cardmethod.CardHolderCity), "data[7][value]");
            form.Add(new StringContent(store.CompanyName), "data[8][value]");
            form.Add(new StringContent(store.CompanyCif), "data[9][value]");
            form.Add(new StringContent(store.City), "data[10][value]");

            _logger.LogInformation($"PaymentServices.FillFormCardMethod(). form: {JsonConvert.SerializeObject(form)}");
            return form;
        }

            public async Task<bool> UpdatePaymentCardProcess(SignatureStatus value, Process process)
        {
            // 1.- Get user
            User user = _userRepository.GetCurrentUserByUsername(value.User);
            string usertype = user.Usertype == (int)UserTypes.Business ? AccountType.Business : string.Empty;

            // 2.- Get account from CRM
            AccountProfile account = await _profileRepository.GetAccountAsync(user.Dni, usertype);

            // 3.- Get process from customerportal DB
            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.CardExternalId = process.Card.ExternalId;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have multiple pending process for this externalId", HttpStatusCode.BadRequest, "CardExternalId", "Pending process");

            PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
            {
                ExternalId = process.Card.ExternalId,
                Confirmed = value.Status == "document_completed" ? true : false,
                Channel = "WEBPORTAL"
            };

            PaymentMethodCardConfirmationResponse cardConfirmation;
            if (process.Card.Update == true)
            {
                cardConfirmation = await _paymentRepository.UpdateConfirmChangePaymentMethodCard(confirmation);
            }
            else
            {
                cardConfirmation = await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
            }

            if (cardConfirmation.Status != "success")
            {
                ProcessCard processCard = processes[0].Card;
                processes[0].Card = new ProcessCard()
                {
                    ExternalId = processCard.ExternalId,
                    Status = 1,
                    Update = processCard.Update
                };
                processes[0].ProcessStatus = (int)ProcessStatuses.Canceled;

                _processRepository.Update(processes[0]);
            }
            // Search current Card and set current to false
            CardSearchFilter cardFilter = new CardSearchFilter()
            {
                SmContractCode = process.SmContractCode,
                Current = true,
                Username = process.Username
            };
            List<Card> findCurrentCards = _cardRepository.Find(cardFilter);

            if (findCurrentCards.Count > 0)
            {
                foreach (Card currentCard in findCurrentCards)
                {
                    currentCard.Current = false;
                    _cardRepository.Update(currentCard);
                }
            }

            Card card = _cardRepository.GetByExternalId(process.Card.ExternalId);
            if (card.Id == null)
                throw new ServiceException("Card doesn´t exits", HttpStatusCode.BadRequest, "CardExternalId");

            card.Current = true;
            _cardRepository.Update(card);

            processes[0].ProcessStatus = (int)ProcessStatuses.Accepted;
            _processRepository.Update(processes[0]);

            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(card.Siteid));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, "StoreId");

            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethodByCard(store.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, "SMId");

            account.PaymentMethodId = payMetCRM.PaymentMethodId;
            account.Token = card.Token;
            account.CardNumber = card.Cardnumber;
            account.TokenUpdate = ((int)TokenUpdateTypes.OK).ToString();
            account.TpvSincronizationDate = DateTime.Now.ToString("O");
            account.UpdateToken = true;

            AccountProfile updateAccount = await _profileRepository.UpdateAccountAsync(account);

            if (updateAccount.SmCustomerId == null)
                throw new ServiceException("Error updating account", HttpStatusCode.BadRequest, "SmCustomerId");

            // Update contract
            string smContractCode = process.SmContractCode;
            //Contract contract = await _contractRepository.GetContractAsync(smContractCode);            
            FullContract fullcontract = (await _contractRepository.GetFullContractsBySMCodeAsync(smContractCode)).FirstOrDefault();
            var contract = FullContractToContract.Mapper(fullcontract);

            contract.PaymentMethodId = payMetCRM.PaymentMethodId;
            var paymentName = !String.IsNullOrEmpty(payMetCRM.Description) ? payMetCRM.Description : payMetCRM.Name;

            Contract updateContract = await _contractRepository.UpdateContractAsync(contract);

            // Get template
            EmailTemplate template = GetTemplateByLanguage(store?.CountryCode, EmailTemplateTypes.UpdatePaymentMethod);

            store = stores.Find(x => x.StoreCode.Contains(contract.StoreData.StoreCode));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, "StoreId");
            _logger.LogInformation("Template StoreMail Information", template._id.ToString());
            if (template._id != null)
            {
                Email message = new Email();
                message.EmailFlow = EmailFlowType.UpdatePaymentCard.ToString();
                string storeMail = contract.StoreData.EmailAddress1;
                _logger.LogInformation("Entering StoreMail Information", storeMail);
                if (storeMail == null) throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);
                if (!(_configuration["Environment"] == nameof(EnvironmentTypes.PRO))) storeMail = _configuration["MailStores"];

                message.To.Add(storeMail);
                message.Subject = string.Format(template.subject, user.Name, user.Dni);
                message.Body = string.Format(template.body, user.Name, user.Dni, process.ContractNumber, paymentName);
                _logger.LogInformation("Sending StoreMail Information", storeMail);
                await _mailRepository.Send(message);
            }

            return true;
        }
        public async Task<Card> GetCard(string username, string token)
        {
            // 1. Get token of data card
            // Card card = _cardRepository.GetCurrent(username, smContractCode);
            // if (card.Id == null)
            //     throw new ServiceException("Current Card doesn´t exits", HttpStatusCode.BadRequest, "Username, smContractCode");

            // 2. Get Card from precognis
            PaymentMethodGetCardResponse cardData = await _paymentRepository.GetCard(token);

            if (cardData.status == "error")
                throw new ServiceException("Card data error", HttpStatusCode.BadRequest, "token");

            // 3. Compose response
            Card card = new Card()
            {
                Id = null,
                ExternalId = null,
                Idcustomer = null,
                Siteid = null,
                Token = token,
                Status = cardData.status,
                Message = cardData.message,
                Cardholder = cardData.card_holder,
                Expirydate = cardData.expirydate,
                Typecard = cardData.type,
                Cardnumber = cardData.cardnumber,
                ContractNumber = null,
                SmContractCode = null,
                Username = null,
                Current = true

            };

            return card;

        }

        public async Task<PaymentMethodPayInvoiceResponse> PayInvoice(PaymentMethodPayInvoice payInvoice)
        {
            // 1. Check payInvoice required values
            if (payInvoice.SiteId == null || payInvoice.SiteId == "")
                throw new ServiceException("SiteId is required", HttpStatusCode.BadRequest, FieldNames.SiteId);

            if (payInvoice.SmContractCode == null || payInvoice.SmContractCode == "")
                throw new ServiceException("SmContractCode is required", HttpStatusCode.BadRequest, FieldNames.SMContractCode);

            if (payInvoice.Ourref == null || payInvoice.Ourref == "")
                throw new ServiceException("Ourref is required", HttpStatusCode.BadRequest, FieldNames.Ourref);

            if (payInvoice.Token == null || payInvoice.Token == "")
                throw new ServiceException("Token is required", HttpStatusCode.BadRequest, FieldNames.Token);

            if (payInvoice.Username == null || payInvoice.Username == "")
                throw new ServiceException("Username is required", HttpStatusCode.BadRequest, FieldNames.Username);

            // 2. Get Invoice
            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(payInvoice.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, FieldNames.SMContractCode);

            Invoice inv = invoices.Find(x => x.OurReference.Contains(payInvoice.Ourref));

            if (inv.OurReference == null)
                throw new ServiceException("Invoice not found", HttpStatusCode.BadRequest, FieldNames.Ourreference);

            // 3. Get SmContract
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(payInvoice.SmContractCode);
            string smContractlog = JsonConvert.SerializeObject(smContract);
            _logger.LogInformation("smContractlog:" + smContractlog);

            if (smContract.Customerid == null)
                throw new ServiceException("Contract sm found", HttpStatusCode.BadRequest, FieldNames.SMContractCode);

            // 4. Set data
            payInvoice.Amount = inv.OutStanding;
            payInvoice.IdCustomer = smContract.Customerid;

            // 5. Pay
            string before = JsonConvert.SerializeObject(payInvoice);
            _logger.LogInformation("before:" + before);

            PaymentMethodPayInvoiceResponse payResponse = await _paymentRepository.PayInvoice(payInvoice);
            if (payResponse.result != "00")
                throw new ServiceException("Error payment", HttpStatusCode.BadRequest, FieldNames.result);
            string after = JsonConvert.SerializeObject(payResponse);
            _logger.LogInformation("after:" + after);



            // 6. Get CRM PayMethods
            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(payInvoice.SiteId));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, FieldNames.StoreId);

            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethod(store.StoreId.ToString());
            string payMetCRMlog = JsonConvert.SerializeObject(payMetCRM);
            _logger.LogInformation("payMetCRMlog:" + payMetCRMlog);
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, FieldNames.SMId);

            // 7. MakePayment SM
            MakePayment mPayment = new MakePayment()
            {
                CustomerId = payInvoice.IdCustomer,
                SiteId = payInvoice.SiteId,
                PayMethod = payMetCRM.SMId,
                PayAmount = payInvoice.Amount,
                PayRef = payInvoice.Ourref,
                DocumentId = inv.DocumentId
            };
            bool makePayment = await _contractSMRepository.MakePayment(mPayment);

            string makePaymentlog = JsonConvert.SerializeObject(makePayment);
            _logger.LogInformation("makePaymentlog:" + makePaymentlog);

            return payResponse;
        }

        public async Task<string> PayInvoiceByNewCardLoad(PaymentMethodPayInvoiceNewCard paymentMethod)
        {
            //1. User must exists
            User user = _userRepository.GetCurrentUserByUsername(paymentMethod.Username);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.NotExist);

            // 2. Validate data

            if (string.IsNullOrEmpty(paymentMethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, FieldNames.SMContractCode, ValidationMessages.EmptyFields);

            var store = await _storeRepository.GetStoreAsync(paymentMethod.SiteId);

            // 4. Get data to load card form string
            string usertype = UserInvitationUtils.GetAccountType(user.Usertype);
            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, usertype);
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(paymentMethod.SmContractCode);

            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(paymentMethod.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, FieldNames.SMContractCode);

            Invoice inv = invoices.Find(x => x.OurReference.Contains(paymentMethod.Ourref));

            if (inv.OurReference == null)
                throw new ServiceException("Invoice not found", HttpStatusCode.BadRequest, FieldNames.Ourreference);
            string externalId = Guid.NewGuid().ToString();

            paymentMethod.ExternalId = externalId;
            paymentMethod.Recurrent = false;
            paymentMethod.Nif = userProfile.DocumentNumber;
            paymentMethod.Name = userProfile.Name;
            paymentMethod.Surnames = userProfile.Surname;
            paymentMethod.DocumentId = inv.DocumentId;
            paymentMethod.Amount = inv.Amount;
            paymentMethod.IdCustomer = smContract.Customerid;
            paymentMethod.Url = _configuration["PayInvoiceByNewCardMethodCardResponse"];
            paymentMethod.Language = getCountryLangByLanguage(user.Language);

            checkFieldsPaymentCardLoad(paymentMethod);

            ProcessSearchFilter filter = new ProcessSearchFilter()
            {
                UserName = user.Username,
                SmContractCode = paymentMethod.SmContractCode,
                ProcessType = (int)ProcessTypes.Payment,
                ProcessStatus = (int)ProcessStatuses.Started
            };
            bool result = await CancelProcessByFilter(filter);

            string paymentMethodlog = JsonConvert.SerializeObject(paymentMethod);
            _logger.LogInformation("paymentMethodlog:" + paymentMethod);

            string stringHtml = await _paymentRepository.PayInvoiceNewCard(paymentMethod);

            Pay pay = new Pay()
            {
                ExternalId = externalId,
                Idcustomer = paymentMethod.IdCustomer,
                Siteid = paymentMethod.SiteId,
                Token = null,
                Status = null,
                Message = null,
                SmContractCode = paymentMethod.SmContractCode,
                Username = paymentMethod.Username,
                DocumentId = paymentMethod.DocumentId,
                InvoiceNumber = paymentMethod.Ourref
            };
            bool createPay = await _payRepository.Create(pay);

            Process process = new Process();
            process.Username = paymentMethod.Username;
            process.ProcessType = (int)ProcessTypes.Payment;
            process.ProcessStatus = (int)ProcessStatuses.Started;
            process.ContractNumber = null;
            process.SmContractCode = paymentMethod.SmContractCode;
            process.Pay = new ProcessPay()
            {
                ExternalId = paymentMethod.ExternalId,
                InvoiceNumber = paymentMethod.Ourref
            };
            process.Documents = null;

            await _processRepository.Create(process);

            return stringHtml;
        }

        public async Task<bool> PayInvoiceByNewCardResponse(PaymentMethodPayInvoiceNewCardResponse payRes)
        {
            string payReslog = JsonConvert.SerializeObject(payRes);
            _logger.LogInformation("payReslog:" + payRes);

            // 1. Save pay response in Pay collection
            Pay findPay = _payRepository.GetByExternalId(payRes.ExternalId);

            if (findPay.Id == null)
                throw new ServiceException("Pay doesn´t exist", HttpStatusCode.BadRequest);

            Pay pay = new Pay()
            {
                Id = findPay.Id,
                ExternalId = payRes.ExternalId,
                Idcustomer = findPay.Idcustomer,
                Siteid = payRes.SiteId,
                Token = payRes.Token,
                Status = payRes.Status,
                Message = payRes.Message,
                SmContractCode = findPay.SmContractCode,
                Username = findPay.Username,
                DocumentId = findPay.DocumentId,
                InvoiceNumber = findPay.InvoiceNumber,

            };
            Pay updatePay = _payRepository.Update(pay);

            // Check if exist process ProcessStatuses.Started (Previous step)
            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.PayExternalId = payRes.ExternalId;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more started process for this externalId", HttpStatusCode.BadRequest, "CardExternalId", "Pending process");

            if (processes.Count == 0)
                throw new ServiceException("User don't have started process for this externalId & ProcessType", HttpStatusCode.BadRequest, "CardExternalId", "Started process");


            // 2. Pay verification failed
            if (payRes.Status != "00")
            {
                Process cancelProcess = new Process();
                cancelProcess.Id = processes[0].Id;
                cancelProcess.Username = updatePay.Username;
                cancelProcess.ProcessType = (int)ProcessTypes.Payment;
                cancelProcess.ProcessStatus = (int)ProcessStatuses.Canceled;
                cancelProcess.ContractNumber = null;
                cancelProcess.SmContractCode = updatePay.SmContractCode;
                cancelProcess.Pay = new ProcessPay()
                {
                    ExternalId = findPay.ExternalId,
                    InvoiceNumber = findPay.InvoiceNumber
                };
                cancelProcess.Documents = null;

                _processRepository.Update(cancelProcess);
                throw new ServiceException("Payment error", HttpStatusCode.BadRequest);
            }
            // 3. Get CRM PayMethods
            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(payRes.SiteId));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, FieldNames.StoreId);

            // 4. Get Payment Method from CRM
            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethod(store.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, FieldNames.SMId);

            // 4. Get Invoice
            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(pay.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, FieldNames.SMContractCode);

            Invoice inv = invoices.Find(x => x.OurReference.Contains(pay.InvoiceNumber));

            // 4. MakePayment SM
            MakePayment mPayment = new MakePayment()
            {
                CustomerId = pay.Idcustomer,
                SiteId = payRes.SiteId,
                DocumentId = inv.DocumentId,
                PayMethod = payMetCRM.SMId,
                PayAmount = inv.Amount,
                PayRef = pay.InvoiceNumber.Replace("/", "")
            };

            string pmPaymentlog = JsonConvert.SerializeObject(mPayment);
            _logger.LogInformation("mPayment:" + mPayment);

            bool makePayment = await _contractSMRepository.MakePayment(mPayment);

            string makePaymentlog = JsonConvert.SerializeObject(makePayment);
            _logger.LogInformation("makePayment:" + makePayment);

            Process process = new Process();
            process.Id = processes[0].Id;
            process.Username = updatePay.Username;
            process.ProcessType = (int)ProcessTypes.Payment;
            process.ProcessStatus = (int)ProcessStatuses.Accepted;
            process.ContractNumber = null;
            process.SmContractCode = updatePay.SmContractCode;
            process.Pay = new ProcessPay()
            {
                ExternalId = updatePay.ExternalId,
                InvoiceNumber = updatePay.InvoiceNumber
            };
            process.Documents = null;

            _processRepository.Update(process);

            return true;
        }

        public async Task<string> UpdateCardLoad(PaymentMethodUpdateCardData updateCardData)
        {
            //1. User must exists
            int userType = UserInvitationUtils.GetUserType(updateCardData.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(updateCardData.Dni, userType);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            // 2. Get the udpated token
            AccountProfile account = await _profileRepository.GetAccountByDocumentNumberAsync(user.Dni);
            updateCardData.Token = account.Token;

            // 3. Validate data
            PaymentMethodUpdateCardData cardmethod = (PaymentMethodUpdateCardData)updateCardData;

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "ContractNumber", "Empty fields");

            string externalId = Guid.NewGuid().ToString();
            updateCardData.ExternalId = externalId;
            updateCardData.Url = _configuration["UpdateCardMethodCardResponse"];
            updateCardData.Language = getCountryLangByLanguage(user.Language);

            checkFieldsUpdateCardLoad(updateCardData);

            ProcessSearchFilter filter = new ProcessSearchFilter()
            {
                UserName = user.Username,
                SmContractCode = cardmethod.SmContractCode,
                ProcessType = (int)ProcessTypes.PaymentMethodChangeCard,
                ProcessStatus = (int)ProcessStatuses.Started
            };
            bool result = await CancelProcessByFilter(filter);

            // 4. Call Precognis/openbrabo
            string stringHtml = await _paymentRepository.UpdateCardLoad(updateCardData);

            // 5. Update fields "TokenUpdate to pending & TokenUpdateDate" in CRM Account
            account.TokenUpdate = ((int)TokenUpdateTypes.Pending).ToString();
            account.TokenUpdateDate = account.TpvSincronizationDate = DateTime.Now.ToString("O");
            account.UpdateToken = false;
            account = await _profileRepository.UpdateAccountAsync(account);

            Card card = new Card()
            {
                SmContractCode = cardmethod.SmContractCode,
                ContractNumber = cardmethod.ContractNumber,
                ExternalId = externalId,
                Username = user.Username,
                Update = true
            };
            bool createCard = await _cardRepository.Create(card);
            if (createCard == false)
                throw new ServiceException("Error creating card", HttpStatusCode.BadRequest);

            Process process = new Process();
            process.Username = user.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
            process.ProcessStatus = (int)ProcessStatuses.Started;
            process.ContractNumber = cardmethod.ContractNumber;
            process.SmContractCode = cardmethod.SmContractCode;
            process.Card = new ProcessCard()
            {
                ExternalId = cardmethod.ExternalId,
                Status = 0,
                Email = cardmethod.Email,
                PhoneNumber = cardmethod.PhoneNumber,
                Address = cardmethod.Address,
                Update = true
            };
            process.Documents = null;

            await _processRepository.Create(process);

            return stringHtml;
        }

        public async Task<bool> UpdateCardResponseAsync(PaymentMethodUpdateCardResponse updateCardResponse)
        {
            // 0. Guardar paymentMethodData en colección Cards
            Card findCard = _cardRepository.GetByExternalId(updateCardResponse.externalid);

            if (findCard.Id == null)
            {
                PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
                {
                    ExternalId = updateCardResponse.externalid,
                    Confirmed = false,
                    Channel = "WEBPORTAL"
                };

                await _paymentRepository.UpdateConfirmChangePaymentMethodCard(confirmation);

                throw new ServiceException("Card doesn´t exist", HttpStatusCode.BadRequest);
            }

            Card card = new Card()
            {
                Id = findCard.Id,
                ExternalId = updateCardResponse.externalid,
                Idcustomer = updateCardResponse.IdCustomer,
                Siteid = updateCardResponse.siteid,
                Token = updateCardResponse.token,
                Status = updateCardResponse.status,
                Message = updateCardResponse.message,
                Cardholder = updateCardResponse.cardholder,
                Expirydate = updateCardResponse.expirydate,
                Typecard = updateCardResponse.typecard,
                Cardnumber = updateCardResponse.cardnumber,
                ContractNumber = findCard.ContractNumber,
                SmContractCode = findCard.SmContractCode,
                Username = findCard.Username,
                Current = false,
                Update = true

            };
            Card updateCard = _cardRepository.Update(card);

            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.CardExternalId = updateCard.ExternalId;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more started process for this externalId", HttpStatusCode.BadRequest, "CardExternalId", "Pending process");

            // Card verification failed
            if (updateCardResponse.status != "00")
            {
                Process cancelProcess = new Process(); ;
                cancelProcess.Username = updateCard.Username;
                cancelProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
                cancelProcess.ProcessStatus = (int)ProcessStatuses.Canceled;
                cancelProcess.ContractNumber = updateCard.ContractNumber;
                cancelProcess.SmContractCode = updateCard.SmContractCode;
                cancelProcess.Card = new ProcessCard()
                {
                    ExternalId = updateCard.ExternalId,
                    Status = 0,
                    Update = true
                };
                cancelProcess.Documents = null;

                await _processRepository.Create(cancelProcess);
                throw new ServiceException("Error card verification", HttpStatusCode.BadRequest);
            }
            if (processes.Count == 1 && processes[0].ProcessStatus == (int)ProcessStatuses.Started)
            {
                PaymentMethodCardSignature paymentMethodCardSignature = await GetPaymentMethodCardSignature(processes[0], card);
                bool result = await ChangePaymentMethodCard(paymentMethodCardSignature);

                return result;
            }

            return false;
        }

        private void checkFieldsCardLoad(PaymentMethodCard cardmethod)
        {
            if (string.IsNullOrEmpty(cardmethod.SiteId))
                throw new ServiceException("Site Id field can not be null.", HttpStatusCode.BadRequest, FieldNames.SiteId, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, FieldNames.ContractNumber, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.Email))
                throw new ServiceException("Email field can not be null.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.EmptyFields);

            if (cardmethod.Email.Length > 254)
                throw new ServiceException("Email field must not be longer to 254.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhoneNumber))
                throw new ServiceException("Phone number can not be null.", HttpStatusCode.BadRequest, FieldNames.PhoneNumber, ValidationMessages.EmptyFields);

            if (cardmethod.PhoneNumber.Length > 15)
                throw new ServiceException("Phone number field must not be longer to 15.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.Street1))
                throw new ServiceException("Street1 can not be null.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.EmptyFields);

            if (cardmethod.Address.Street1.Length > 50)
                throw new ServiceException("Street1 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street2.Length > 50)
                throw new ServiceException("Street2 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street3.Length > 50)
                throw new ServiceException("Street3 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.ZipOrPostalCode))
                throw new ServiceException("Zip Or Postal Code can not be null.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.EmptyFields);

            if (cardmethod.Address.ZipOrPostalCode.Length > 16)
                throw new ServiceException("Zip Or Postal Code field must not be longer to 16.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.City))
                throw new ServiceException("City can not be null.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.EmptyFields);

            if (cardmethod.Address.City.Length > 50)
                throw new ServiceException("City field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.CountryISOCodeNumeric))
                throw new ServiceException("Country ISO Code Numeric can not be null.", HttpStatusCode.BadRequest, FieldNames.CountryISOCodeNumeric, ValidationMessages.EmptyFields);

            if (cardmethod.CountryISOCodeNumeric.Length > 3)
                throw new ServiceException("Country ISO Code Numeric field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhonePrefix))
                throw new ServiceException("Phone Prefix can not be null.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.EmptyFields);

            if (cardmethod.PhonePrefix.Length > 3)
                throw new ServiceException("Phone Prefix field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.IdCustomer))
                throw new ServiceException("Id Customer can not be null.", HttpStatusCode.BadRequest, FieldNames.IdCustomer, ValidationMessages.EmptyFields);

        }
        private void checkFieldsUpdateCardLoad(PaymentMethodUpdateCardData cardmethod)
        {
            if (string.IsNullOrEmpty(cardmethod.SiteId))
                throw new ServiceException("Site Id field can not be null.", HttpStatusCode.BadRequest, FieldNames.SiteId, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, FieldNames.ContractNumber, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.Email))
                throw new ServiceException("Email field can not be null.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.EmptyFields);

            if (cardmethod.Email.Length > 254)
                throw new ServiceException("Email field must not be longer to 254.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhoneNumber))
                throw new ServiceException("Phone number can not be null.", HttpStatusCode.BadRequest, FieldNames.PhoneNumber, ValidationMessages.EmptyFields);

            if (cardmethod.PhoneNumber.Length > 15)
                throw new ServiceException("Phone number field must not be longer to 15.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.Street1))
                throw new ServiceException("Street1 can not be null.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.EmptyFields);

            if (cardmethod.Address.Street1.Length > 50)
                throw new ServiceException("Street1 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street2.Length > 50)
                throw new ServiceException("Street2 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street3.Length > 50)
                throw new ServiceException("Street3 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.ZipOrPostalCode))
                throw new ServiceException("Zip Or Postal Code can not be null.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.EmptyFields);

            if (cardmethod.Address.ZipOrPostalCode.Length > 16)
                throw new ServiceException("Zip Or Postal Code field must not be longer to 16.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.City))
                throw new ServiceException("City can not be null.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.EmptyFields);

            if (cardmethod.Address.City.Length > 50)
                throw new ServiceException("City field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.CountryISOCodeNumeric))
                throw new ServiceException("Country ISO Code Numeric can not be null.", HttpStatusCode.BadRequest, FieldNames.CountryISOCodeNumeric, ValidationMessages.EmptyFields);

            if (cardmethod.CountryISOCodeNumeric.Length > 3)
                throw new ServiceException("Country ISO Code Numeric field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhonePrefix))
                throw new ServiceException("Phone Prefix can not be null.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.EmptyFields);

            if (cardmethod.PhonePrefix.Length > 3)
                throw new ServiceException("Phone Prefix field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.IdCustomer))
                throw new ServiceException("Id Customer can not be null.", HttpStatusCode.BadRequest, FieldNames.IdCustomer, ValidationMessages.EmptyFields);

        }

        private async Task<bool> CancelProcessByFilter(ProcessSearchFilter filter)
        {
            List<Process> processes = _processRepository.Find(filter);
            if (processes.Count > 0)
            {
                Process pro = processes[0];
                pro.ProcessStatus = (int)ProcessStatuses.Canceled;
                _processRepository.Update(pro);

                string externalId = null;

                // Change payment method
                if (!string.IsNullOrEmpty(pro.Id) && pro.Card != null && !string.IsNullOrEmpty(pro.Card.ExternalId))
                {
                    externalId = pro.Card.ExternalId;
                }

                // Paymen invoice
                if (string.IsNullOrEmpty(externalId) && pro.Pay != null && !string.IsNullOrEmpty(pro.Id) && !string.IsNullOrEmpty(pro.Pay.ExternalId))
                {
                    externalId = pro.Pay.ExternalId;
                }

                // Cancel new card o exist card on Precognis
                if (externalId != null)
                {

                    PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
                    {
                        ExternalId = externalId,
                        Channel = "WEBPORTAL",
                        Confirmed = false
                    };
                    await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
                    await _paymentRepository.UpdateConfirmChangePaymentMethodCard(confirmation);
                    return true;
                }

            }

            return false;
        }

        private async Task<PaymentMethodCardSignature> GetPaymentMethodCardSignature(Process process, Card card)
        {
            User user = _userRepository.GetCurrentUserByUsername(process.Username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.NotExist);

            Contract contract = await _contractRepository.GetContractAsync(process.SmContractCode);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");

            string address = process.Card.Address.Street1 + " " + process.Card.Address.Street2 + process.Card.Address.Street3;
            PaymentMethodCardSignature paymentMethodCardSignature = new PaymentMethodCardSignature
            {
                ContractNumber = process.ContractNumber,
                SmContractCode = process.SmContractCode,
                UnitNumber = contract.Unit.UnitName,
                CardHolderName = card.Cardholder,
                CardHolderCif = user.Dni,
                CardHolderAddress = address,
                CardHolderPostalCode = process.Card.Address.ZipOrPostalCode,
                CardHolderCity = process.Card.Address.City,
                SiteId = card.Siteid,
                ExternalId = card.ExternalId,
                Username = process.Username,
                PaymentMethodType = (int)PaymentMethodTypes.CreditCard,
                AccountType = UserInvitationUtils.GetAccountType(user.Usertype)
            };

            return paymentMethodCardSignature;
        }

        private void checkFieldsPaymentCardLoad(PaymentMethodPayInvoiceNewCard cardmethod)
        {
            if (string.IsNullOrEmpty(cardmethod.SiteId))
                throw new ServiceException("Site Id field can not be null.", HttpStatusCode.BadRequest, FieldNames.SiteId, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, FieldNames.ContractNumber, ValidationMessages.EmptyFields);

            if (string.IsNullOrEmpty(cardmethod.Email))
                throw new ServiceException("Email field can not be null.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.EmptyFields);

            if (cardmethod.Email.Length > 254)
                throw new ServiceException("Email field must not be longer to 254.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhoneNumber))
                throw new ServiceException("Phone number can not be null.", HttpStatusCode.BadRequest, FieldNames.PhoneNumber, ValidationMessages.EmptyFields);

            if (cardmethod.PhoneNumber.Length > 15)
                throw new ServiceException("Phone number field must not be longer to 15.", HttpStatusCode.BadRequest, FieldNames.Email, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.Street1))
                throw new ServiceException("Street1 can not be null.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.EmptyFields);

            if (cardmethod.Address.Street1.Length > 50)
                throw new ServiceException("Street1 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street2.Length > 50)
                throw new ServiceException("Street2 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (cardmethod.Address.Street3.Length > 50)
                throw new ServiceException("Street3 field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.Street, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.ZipOrPostalCode))
                throw new ServiceException("Zip Or Postal Code can not be null.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.EmptyFields);

            if (cardmethod.Address.ZipOrPostalCode.Length > 16)
                throw new ServiceException("Zip Or Postal Code field must not be longer to 16.", HttpStatusCode.BadRequest, FieldNames.ZipOrPostalCode, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.Address.City))
                throw new ServiceException("City can not be null.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.EmptyFields);

            if (cardmethod.Address.City.Length > 50)
                throw new ServiceException("City field must not be longer to 50.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.CountryISOCodeNumeric))
                throw new ServiceException("Country ISO Code Numeric can not be null.", HttpStatusCode.BadRequest, FieldNames.CountryISOCodeNumeric, ValidationMessages.EmptyFields);

            if (cardmethod.CountryISOCodeNumeric.Length > 3)
                throw new ServiceException("Country ISO Code Numeric field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.CountryISOCodeNumeric, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.PhonePrefix))
                throw new ServiceException("Phone Prefix can not be null.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.EmptyFields);

            if (cardmethod.PhonePrefix.Length > 3)
                throw new ServiceException("Phone Prefix field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.PhonePrefix, ValidationMessages.LongerTo);

            if (string.IsNullOrEmpty(cardmethod.IdCustomer))
                throw new ServiceException("Id Customer can not be null.", HttpStatusCode.BadRequest, FieldNames.IdCustomer, ValidationMessages.EmptyFields);

        }

        public async Task<List<PaymentMethods>> GetAvailablePaymentMethods(string smContractCode)
        {
            // 1. Get Contract
            Contract contract = await _contractRepository.GetContractAsync(smContractCode);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, FieldNames.ContractNumber, ValidationMessages.NotExist);

            // 2. Get CRM PayMethods
            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Equals(contract.StoreData.StoreCode));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, FieldNames.StoreId);

            PaymentMethodsList payMet = await _paymentMethodRepository.GetAllPaymentMethods(store.StoreId.ToString());
            if (payMet.PaymentMethods == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, FieldNames.SMId);

            List<PaymentMethods> availablePayMet = new List<PaymentMethods>();

            var bankAccountMethodList = payMet.PaymentMethods.Where(a => a.BankAccountPayment == true).FirstOrDefault();
            var cardAccountMethodList = payMet.PaymentMethods.Where(a => a.CardPayment == true).FirstOrDefault();

            if (bankAccountMethodList != null)
            {
                availablePayMet.Add(bankAccountMethodList);
            }
            if (cardAccountMethodList != null)
            {
                availablePayMet.Add(cardAccountMethodList);
            }

            return availablePayMet;
        }
    }
}
