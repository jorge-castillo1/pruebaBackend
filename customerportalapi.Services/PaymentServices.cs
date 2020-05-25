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
            IPayRepository payRepository
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
            SMBankAccount bankAccount = new SMBankAccount();
            User user = _userRepository.GetCurrentUser(value.User);
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

            bankAccount.CustomerId = account.SmCustomerId;
            bankAccount.PaymentMethodId = "AT5";
            bankAccount.AccountName = processedpaymentdocument.BankAccountName;
            bankAccount.AccountNumber = processedpaymentdocument.BankAccountOrderNumber;
            bankAccount.Default = 1;
            bankAccount.Iban = processedpaymentdocument.BankAccountOrderNumber;
            await _accountSMRepository.AddBankAccountAsync(bankAccount);

            // Send email to the store
            EmailTemplate template = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.UpdateBankAccount, LanguageTypes.en.ToString());
            string smContractCode = processedpaymentdocument.SmContractCode;
            Contract contract = await _contractRepository.GetContractAsync(smContractCode);

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

            form.Add(new StringContent(store.StoreName), "storeidentification");
            form.Add(new StringContent(SystemTypes.CustomerPortal.ToString()), "sourcesystem");
            form.Add(new StringContent(user.Username), "sourceuser");
            form.Add(new StringContent(user.Usertype.ToString()), "accounttype");
            form.Add(new StringContent(user.Dni.ToString()), "accountdni");
            form.Add(new StringContent(_configuration["GatewaySignatureEventsUrl"]), "signatureendprocess_url");

            // data
            form.Add(new StringContent("contractnumber"), "data[0][key]");
            form.Add(new StringContent("company"), "data[1][key]");
            form.Add(new StringContent("cif"), "data[2][key]");
            form.Add(new StringContent("accountname"), "data[3][key]");
            form.Add(new StringContent("address"), "data[4][key]");
            form.Add(new StringContent("postalcode"), "data[5][key]");
            form.Add(new StringContent("country"), "data[6][key]");
            form.Add(new StringContent("clientname"), "data[7][key]");
            form.Add(new StringContent("clientaddress"), "data[8][key]");
            form.Add(new StringContent("clientpostalcode"), "data[9][key]");
            form.Add(new StringContent("clientcountry"), "data[10][key]");
            form.Add(new StringContent("iban"), "data[11][key]");
            form.Add(new StringContent(bankmethod.ContractNumber), "data[0][value]");
            form.Add(new StringContent(store.CompanyName), "data[1][value]");
            form.Add(new StringContent(store.CompanyCif), "data[2][value]");
            form.Add(new StringContent(store.CompanyName), "data[3][value]");
            form.Add(new StringContent(store.CompanySocialAddress), "data[4][value]");
            form.Add(new StringContent("00000"), "data[5][value]");
            form.Add(new StringContent(store.Country), "data[6][value]");
            form.Add(new StringContent(bankmethod.FullName), "data[7][value]");
            form.Add(new StringContent(bankmethod.Address), "data[8][value]");
            form.Add(new StringContent(bankmethod.PostalCode), "data[9][value]");
            form.Add(new StringContent(bankmethod.Country), "data[10][value]");
            form.Add(new StringContent(bankmethod.IBAN), "data[11][value]");
            return form;

        }

        public async Task<string> ChangePaymentMethodCardLoad(PaymentMethodCard paymentMethod)
        {
             //1. User must exists
            int userType = UserUtils.GetUserType(paymentMethod.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(paymentMethod.Dni, userType);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            // 2. Validate data
            PaymentMethodCard cardmethod = (PaymentMethodCard)paymentMethod;

            if (string.IsNullOrEmpty(cardmethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "ContractNumber", "Empty fields");
            
            var store = await _storeRepository.GetStoreAsync(cardmethod.SiteId);
            
            // 4. Get data to load card form string 

            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, paymentMethod.AccountType);
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(cardmethod.SmContractCode);
            string externalId = Guid.NewGuid().ToString();

            HttpContent content = FillFormUrlEncodedCardMethod(store, userProfile, smContract, externalId);

            string stringHtml = await _paymentRepository.ChangePaymentMethodCard(content);
            Card card = new Card()
            {
                SmContractCode = cardmethod.SmContractCode,
                ContractNumber = cardmethod.ContractNumber,
                ExternalId = externalId,
                Username = user.Username
            };
            bool createCard = await _cardRepository.Create(card);
            if (createCard == false)
                throw new ServiceException("Error creating card", HttpStatusCode.BadRequest);

            return stringHtml;
        }
        
        private HttpContent FillFormUrlEncodedCardMethod(Store store, Profile user, SMContract smContract, string externalId)
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("externalid", externalId));
            keyValues.Add(new KeyValuePair<string, string>("channel", "WEBPORTAL"));
            keyValues.Add(new KeyValuePair<string, string>("siteid", store.StoreCode));
            keyValues.Add(new KeyValuePair<string, string>("name", user.Name));
            keyValues.Add(new KeyValuePair<string, string>("surnames", user.Surname));
            keyValues.Add(new KeyValuePair<string, string>("nif", user.DocumentNumber));
            keyValues.Add(new KeyValuePair<string, string>("idcustomer", smContract.Customerid));
            keyValues.Add(new KeyValuePair<string, string>("url", _configuration["ChangePaymentMethodCardResponse"]));
            HttpContent content = new FormUrlEncodedContent(keyValues);
            return content;

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
            searchProcess.ProcessStatus = (int)ProcessStatuses.Pending;
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
            
            Process process = new Process();
            process.Username = updateCard.Username;
            process.ProcessType = (int)ProcessTypes.PaymentMethodChangeCard;
            process.ProcessStatus = (int)ProcessStatuses.Pending;
            process.ContractNumber = updateCard.ContractNumber;
            process.SmContractCode = updateCard.SmContractCode;
            process.Card = new ProcessCard()
            {
                ExternalId = updateCard.ExternalId,
                Status = 0
            };
            process.Documents = null;

            await _processRepository.Create(process);
            
            return true;
        }
        public async Task<bool> ChangePaymentMethodCard(PaymentMethodCardSignature paymentMethodCardSignature)
        {
            //1. User must exists
            PaymentMethodCardSignature cardmethod = (PaymentMethodCardSignature)paymentMethodCardSignature;
            int userType = UserUtils.GetUserType(cardmethod.AccountType);
            User user = _userRepository.GetCurrentUser(cardmethod.Username);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");
           
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
            searchProcess.ProcessStatus = (int)ProcessStatuses.Pending;
            searchProcess.ExternalId = cardmethod.ExternalId;
            List<Process> processes = _processRepository.Find(searchProcess);
            if (processes.Count > 1)
                throw new ServiceException("User have two or more pending process for this externalId", HttpStatusCode.BadRequest, "ExternalId", "Pending process");


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
                Status = 1
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
            User user = _userRepository.GetCurrentUser(value.User);
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
           
            PaymentMethodCardConfirmationResponse cardConfirmation = await _paymentRepository.ConfirmChangePaymentMethodCard(confirmation);
            if (cardConfirmation.Status != "success") {
                ProcessCard processCard = processes[0].Card;
                processes[0].Card = new ProcessCard()
                {
                    ExternalId = processCard.ExternalId,
                    Status = 1
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
           
            return true;
        }
        public async Task<Card> GetCard(string username, string smContractCode)
        {
            // 1. Get token of data card
            Card card = _cardRepository.GetCurrent(username, smContractCode);
            if (card.Id == null)
                throw new ServiceException("Current Card doesn´t exits", HttpStatusCode.BadRequest, "Username, smContractCode");
            
            // 2. Get Card from precognis
            PaymentMethodGetCardResponse cardData = await _paymentRepository.GetCard(card.Token);

            if (cardData.status == "error")
                throw new ServiceException("Card data error", HttpStatusCode.BadRequest, "token");
            
            // 3. Update own card data to prevent discrepancies
             Card cardToUpdate = new Card() 
            {
                Id = card.Id,
                ExternalId = card.ExternalId,
                Idcustomer = card.Idcustomer,
                Siteid = card.Siteid,
                Token = card.Token,
                Status = card.Status,
                Message = card.Message,
                Cardholder = cardData.card_holder,
                Expirydate = cardData.expirydate,
                Typecard = cardData.type,
                Cardnumber = cardData.cardnumber,
                ContractNumber = card.ContractNumber,
                SmContractCode = card.SmContractCode,
                Username = card.Username,
                Current = card.Current

            };
            Card updatedCard = _cardRepository.Update(cardToUpdate);

            // 4. 
            return updatedCard;

        }

        public async Task<PaymentMethodPayInvoiceResponse> PayInvoice(PaymentMethodPayInvoice payInvoice)
        {
            // 1. Check payInvoice required values
            if (payInvoice.SiteId == null || payInvoice.SiteId == "")
                throw new ServiceException("SiteId is required", HttpStatusCode.BadRequest, "SiteId");

            if (payInvoice.SmContractCode == null || payInvoice.SmContractCode == "")
                throw new ServiceException("SmContractCode is required", HttpStatusCode.BadRequest, "SmContractCode");

            if (payInvoice.Ourref == null || payInvoice.Ourref == "")
                throw new ServiceException("Ourref is required", HttpStatusCode.BadRequest, "Ourref");

            if (payInvoice.Token == null || payInvoice.Token == "")
                throw new ServiceException("Token is required", HttpStatusCode.BadRequest, "Token");
            
            if (payInvoice.Username == null || payInvoice.Username == "")
                throw new ServiceException("Username is required", HttpStatusCode.BadRequest, "Username");
            
            // 2. Get Invoice
            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(payInvoice.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, "smContractCode");

               Invoice inv = invoices.Find(x => x.OurReference.Contains(payInvoice.Ourref));

            if (inv.OurReference == null)
                throw new ServiceException("Invoice not found", HttpStatusCode.BadRequest, "Ourreference");

            // 3. Get SmContract 
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(payInvoice.SmContractCode);

            if (smContract.Customerid == null)
                throw new ServiceException("Contract sm found", HttpStatusCode.BadRequest, "SmContractCode");

            // 4. Set data
            payInvoice.Amount = inv.Amount;
            payInvoice.IdCustomer = smContract.Customerid;

            // 5. Pay
            PaymentMethodPayInvoiceResponse payResponse = await _paymentRepository.PayInvoice(payInvoice);
            if (payResponse.result != "00")
                throw new ServiceException("Error payment", HttpStatusCode.BadRequest, "result");

            // 6. Get CRM PayMethods
            List<Store> stores = await _storeRepository.GetStoresAsync();
            Store store = stores.Find(x => x.StoreCode.Contains(payInvoice.SiteId));
            if (store.StoreId == null)
                throw new ServiceException("Store not found", HttpStatusCode.BadRequest, "StoreId");

            PaymentMethodCRM payMetCRM = await _paymentMethodRepository.GetPaymentMethod(store.StoreId.ToString());
            if (payMetCRM.SMId == null)
                throw new ServiceException("Error payment method crm", HttpStatusCode.BadRequest, "SMId");
        
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

            return payResponse;
        }

        public async Task<string> PayInvoiceByNewCardLoad(PaymentMethodPayInvoiceNewCard paymentMethod)
        {
             //1. User must exists
            User user = _userRepository.GetCurrentUser (paymentMethod.Username);

            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            // 2. Validate data

            if (string.IsNullOrEmpty(paymentMethod.SmContractCode))
                throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "SMContractCode", "Empty fields");
            
            var store = await _storeRepository.GetStoreAsync(paymentMethod.SiteId);
            
            // 4. Get data to load card form string 
            string usertype = UserUtils.GetAccountType(user.Usertype);
            Profile userProfile = await _profileRepository.GetProfileAsync(user.Dni, usertype);
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(paymentMethod.SmContractCode);

            List<Invoice> invoices = await _contractSMRepository.GetInvoicesAsync(paymentMethod.SmContractCode);
            if (invoices.Count <= 0)
                throw new ServiceException("Invoices not found", HttpStatusCode.BadRequest, "smContractCode");

               Invoice inv = invoices.Find(x => x.OurReference.Contains(paymentMethod.Ourref));

            if (inv.OurReference == null)
                throw new ServiceException("Invoice not found", HttpStatusCode.BadRequest, "Ourreference");
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
    }    
}
