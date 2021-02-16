using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            int userType = UserUtils.GetUserType(paymentMethod.AccountType);
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

            //4. Generate and send Document To SignatureAPI

            var store = await _storeRepository.GetStoreAsync(bankmethod.StoreCode);

            var form = FillFormBankMethod(store, bankmethod, user);
            Guid documentid = await _signatureRepository.CreateSignature(form);

            //5. Create a change method payment process
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

        public async Task<bool> UpdatePaymentProcess(SignatureStatus value)
        {
            // Add Bank account to SM
            //


            User user = _userRepository.GetCurrentUserByUsername(value.User);
            string usertype = user.Usertype == (int)UserTypes.Business ? AccountType.Business : string.Empty;
            AccountProfile account = await _profileRepository.GetAccountAsync(user.Dni, usertype);

            //Get signature data
            SignatureSearchFilter filter = new SignatureSearchFilter();
            filter.Filters.SignatureId = value.SignatureId;
            List<SignatureProcess> signatures = await _signatureRepository.SearchSignaturesAsync(filter);
            if (signatures.Count != 1)
                throw new ServiceException("Error searching signature for this process", HttpStatusCode.BadRequest, "SignatureId", "Not exist");

            ProcessedDocument processedpaymentdocument = null;
            var docIndex = 0;
            foreach(SignatureDocumentResult dr in signatures[0].SignatureResult.Documents)
            {
                if (dr.Id.ToString() == value.DocumentId)
                    processedpaymentdocument = signatures[0].Documents[docIndex];
            }

            // bankAccount.CustomerId = account.SmCustomerId;
            // bankAccount.PaymentMethodId = "AT5";
            // bankAccount.AccountName = processedpaymentdocument.BankAccountName;
            // bankAccount.AccountNumber = processedpaymentdocument.BankAccountOrderNumber;
            // bankAccount.Default = 1;
            // bankAccount.Iban = processedpaymentdocument.BankAccountOrderNumber;
            // await _accountSMRepository.AddBankAccountAsync(bankAccount);

            // Send email to the store
            EmailTemplate template = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.UpdateBankAccount, LanguageTypes.en.ToString());
            string smContractCode = processedpaymentdocument.SmContractCode;
            Contract contract = await _contractRepository.GetContractAsync(smContractCode);

            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(contract.StoreData.StoreCode));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, "StoreId");

            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethodByBankAccount(store.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, "SMId");

            account.BankAccount = processedpaymentdocument.BankAccountOrderNumber;
            AccountProfile updateAccount = await _profileRepository.UpdateAccountAsync(account);

            contract.PaymentMethod = payMetCRM.PaymentMethodId;
            Contract updateContract = await _contractRepository.UpdateContractAsync(contract);


            if (updateAccount.SmCustomerId == null)
                throw new ServiceException("Error updating account", HttpStatusCode.BadRequest, "SmCustomerId");

            if (template._id != null)
            {
                Email message = new Email();
                string storeMail = contract.StoreData.EmailAddress1;
                if (storeMail == null) throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);
                if (!(_configuration["Environment"] == nameof(EnvironmentTypes.PRO))) storeMail = _configuration["MailStores"];
                message.To.Add(storeMail);
                message.Subject = string.Format(template.subject, user.Name, user.Dni);
                message.Body = string.Format(template.body, user.Name, user.Dni, processedpaymentdocument.DocumentNumber);
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
            return form;

        }

        public async Task<string> ChangePaymentMethodCardLoad(PaymentMethodCard paymentMethod)
        {
             //1. User must exists
            int userType = UserUtils.GetUserType(paymentMethod.AccountType);
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
                if(!string.IsNullOrEmpty(smContract.Customerid))
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
            HttpContent content = new FormUrlEncodedContent(keyValues);
            return content;
        }

        private string getCountryLangByLanguage(string language) {
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
            // 0. Guardar paymentMethodData en colección Cards
            Card findCard = _cardRepository.GetByExternalId(cardData.ExternalId);

            if (findCard.Id == null) {
                PaymentMethodCardConfirmationToken confirmation  = new PaymentMethodCardConfirmationToken()
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
            searchProcess.ExternalId = updateCard.ExternalId;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more pending process for this externalId", HttpStatusCode.BadRequest, "ExternalId", "Pending process");


            // Card verification failed
            if (cardData.Status != "00") {
                Process cancelProcess = new Process();;
                cancelProcess.Username = updateCard.Username;
                cancelProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
                cancelProcess.ProcessStatus = (int)ProcessStatuses.Canceled;
                cancelProcess.ContractNumber = updateCard.ContractNumber;
                cancelProcess.SmContractCode = updateCard.SmContractCode;
                cancelProcess.Card = new ProcessCard()
                {
                    ExternalId = updateCard.ExternalId,
                    Status = 0
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
        public async Task<bool> ChangePaymentMethodCard(PaymentMethodCardSignature paymentMethodCardSignature)
        {
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
            if (card.Id == null) {
                PaymentMethodCardConfirmationToken confirmation  = new PaymentMethodCardConfirmationToken()
                {
                    ExternalId = cardmethod.ExternalId,
                    Confirmed = false,
                    Channel = "WEBPORTAL"
                };

                PaymentMethodCardConfirmationResponse cardConfirmation = await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
                throw new ServiceException("Card not found", HttpStatusCode.BadRequest, "ExternalId");
            }

            cardmethod.CardHolderName = card.Cardholder;
            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, cardmethod.AccountType);

            var form = FillFormCardMethod(store, cardmethod, user, userProfile);
            Guid documentid = await _signatureRepository.CreateSignature(form);

            // 2. Update process confirmCardSignature
            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.ExternalId = cardmethod.ExternalId;
            searchProcess.UserName = user.Username;
            searchProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
            searchProcess.SmContractCode = cardmethod.SmContractCode;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            searchProcess.ExternalId = cardmethod.ExternalId;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more started process for this externalId & ProcessType", HttpStatusCode.BadRequest, "ExternalId", "Started process");

            if (processes.Count == 0)
                throw new ServiceException("User don't have started process for this externalId & ProcessType", HttpStatusCode.BadRequest, "ExternalId", "Started process");


            Process process = new Process();
            process.Id = processes[0].Id;
            process.Username = user.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeCardSignature;
            process.ProcessStatus = (int)ProcessStatuses.Pending;
            process.ContractNumber = cardmethod.ContractNumber;
            process.SmContractCode = cardmethod.SmContractCode;
            process.CreationDate = processes[0].CreationDate;
            process.ModifiedDate = System.DateTime.Now;
            process.Card = new ProcessCard()
            {
                ExternalId = cardmethod.ExternalId,
                Status = 1,
                Update = processes[0].Card.Update
            };
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
            return form;
        }

        public async Task<bool> UpdatePaymentCardProcess(SignatureStatus value, Process process)
        {
            // Get user
            User user = _userRepository.GetCurrentUserByUsername(value.User);
            string usertype = user.Usertype == (int)UserTypes.Business ? AccountType.Business : string.Empty;
            AccountProfile account = await _profileRepository.GetAccountAsync(user.Dni, usertype);

            ProcessSearchFilter searchProcess = new ProcessSearchFilter();
            searchProcess.ExternalId = process.Card.ExternalId;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have multiple pending process for this externalId", HttpStatusCode.BadRequest, "ExternalId", "Pending process");

            PaymentMethodCardConfirmationToken confirmation  = new PaymentMethodCardConfirmationToken()
            {
                ExternalId = process.Card.ExternalId,
                Confirmed = value.Status == "document_completed" ? true : false,
                Channel = "WEBPORTAL"
            };

            PaymentMethodCardConfirmationResponse cardConfirmation;
            if (process.Card.Update == true) {
                cardConfirmation = await _paymentRepository.UpdateConfirmChangePaymentMethodCard(confirmation);
            } else {
                cardConfirmation = await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
            }

            if (cardConfirmation.Status != "success") {
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
            CardSearchFilter cardFilter = new CardSearchFilter() {
                SmContractCode = process.SmContractCode,
                Current = true,
                Username = process.Username
            };
            List<Card> findCurrentCards = _cardRepository.Find(cardFilter);

            if (findCurrentCards.Count > 0) {
                foreach (Card currentCard in findCurrentCards)
                {
                    currentCard.Current = false;
                    _cardRepository.Update(currentCard);
                }
            }

            Card card = _cardRepository.GetByExternalId(process.Card.ExternalId);
            if (card.Id == null)
                throw new ServiceException("Card doesn´t exits", HttpStatusCode.BadRequest, "ExternalId");

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

            account.Token = card.Token;
            account.TokenUpdateDate = DateTime.UtcNow.ToString("O");
            AccountProfile updateAccount = await _profileRepository.UpdateAccountAsync(account);

            if (updateAccount.SmCustomerId == null)
                throw new ServiceException("Error updating account", HttpStatusCode.BadRequest, "SmCustomerId");

            // Update contract
            string smContractCode = process.SmContractCode;
            Contract contract = await _contractRepository.GetContractAsync(smContractCode);
            contract.PaymentMethod = payMetCRM.PaymentMethodId;
            Contract updateContract = await _contractRepository.UpdateContractAsync(contract);

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
            payInvoice.Amount = inv.Amount;
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
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.EmptyFields);

            // 2. Validate data

            if (string.IsNullOrEmpty(paymentMethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, FieldNames.SMContractCode, ValidationMessages.EmptyFields);

            var store = await _storeRepository.GetStoreAsync(paymentMethod.SiteId);

            // 4. Get data to load card form string
            string usertype = UserUtils.GetAccountType(user.Usertype);
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
            paymentMethod.DocumentId = paymentMethod.Ourref;
            paymentMethod.Amount = inv.Amount;
            paymentMethod.IdCustomer = smContract.Customerid;
            paymentMethod.Url =  _configuration["PayInvoiceByNewCardMethodCardResponse"];
            paymentMethod.Language = getCountryLangByLanguage(user.Language);

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

            return stringHtml;
        }

        public async Task<bool> PayInvoiceByNewCardResponse(PaymentMethodPayInvoiceNewCardResponse payRes)
        {
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

            // 2. Pay verification failed
            if (payRes.Status != "00") {
                Process cancelProcess = new Process();;
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

                await _processRepository.Create(cancelProcess);
                throw new ServiceException("Payment error", HttpStatusCode.BadRequest);
            }

             // 3. Get CRM PayMethods
            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(payRes.SiteId));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, "StoreId");

            // 4. Get Payment Method from CRM
            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethod(store.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, "SMId");

             // 4. Get Invoice
            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(pay.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, "smContractCode");

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
            bool makePayment = await _contractSMRepository.MakePayment(mPayment);

            Process process = new Process();
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

            await _processRepository.Create(process);

            return true;
        }

        public async Task<string> UpdateCardLoad(PaymentMethodUpdateCardData updateCardData)
        {
             //1. User must exists
            int userType = UserUtils.GetUserType(updateCardData.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(updateCardData.Dni, userType);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            // 2. Validate data
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

            string stringHtml = await _paymentRepository.UpdateCardLoad(updateCardData);
            
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
                Address = cardmethod.Address
            };
            process.Documents = null;

            await _processRepository.Create(process);

            return stringHtml;
        }

        public async Task<bool> UpdateCardResponseAsync(PaymentMethodUpdateCardResponse updateCardResponse)
        {
            // 0. Guardar paymentMethodData en colección Cards
            Card findCard = _cardRepository.GetByExternalId(updateCardResponse.externalid);

            if (findCard.Id == null) {
                PaymentMethodCardConfirmationToken confirmation  = new PaymentMethodCardConfirmationToken()
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
            searchProcess.ExternalId = updateCard.ExternalId;
            searchProcess.ProcessStatus = (int)ProcessStatuses.Started;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more pending process for this externalId", HttpStatusCode.BadRequest, "ExternalId", "Pending process");

            // Card verification failed
            if (updateCardResponse.status != "00") {
                Process cancelProcess = new Process();;
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
                throw new ServiceException("City field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

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
                throw new ServiceException("City field must not be longer to 3.", HttpStatusCode.BadRequest, FieldNames.City, ValidationMessages.LongerTo);

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
                ProcessCard card = pro.Card;
                PaymentMethodCardConfirmationToken confirmation = new PaymentMethodCardConfirmationToken()
                {
                    ExternalId = card.ExternalId,
                    Channel = "WEBPORTAL",
                    Confirmed = false
                };
                await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
                await _paymentRepository.UpdateConfirmChangePaymentMethodCard(confirmation);
                return true;
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
                AccountType = UserUtils.GetAccountType(user.Usertype)
            };

            return paymentMethodCardSignature;
        }

    }

}
