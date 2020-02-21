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

namespace customerportalapi.Services
{
    public class PaymentServices : IPaymentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProcessRepository _processRepository;
        private readonly ISignatureRepository _signatureRepository;
        private readonly IStoreRepository _storeRepository;
        public PaymentServices(IUserRepository userRepository, IProcessRepository processRepository, ISignatureRepository signatureRepository, IStoreRepository storeRepository)
        {
            _userRepository = userRepository;
            _processRepository = processRepository;
            _signatureRepository = signatureRepository;
            _storeRepository = storeRepository;
        }

        public async Task<bool> ChangePaymentMethod(PaymentMethod paymentMethod)
        {
            //1. User must exists
            int userType = UserUtils.GetUserType(paymentMethod.AccountType);
            User user = _userRepository.GetCurrentUserByDniAndType(paymentMethod.Dni, userType);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //2. Process paymentmethod change
            if (paymentMethod.PaymentMethodType == (int)PaymentMethodTypes.Bank)
            {
                PaymentMethodBank bankmethod = (PaymentMethodBank)paymentMethod;

                //3. Validate contract number
                if (string.IsNullOrEmpty(bankmethod.ContractNumber))
                    throw new ServiceException("Contract number field can not be null.", HttpStatusCode.BadRequest, "ContractNumber", "Empty fields");

                //4. Validate that don't exist any pending process for same user, process type and contract number
                ProcessSearchFilter searchProcess = new ProcessSearchFilter();
                searchProcess.UserName = user.Username;
                searchProcess.ProcessType = (int)ProcessTypes.PaymentMethodChangeBank;
                searchProcess.ContractNumber = bankmethod.ContractNumber;
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
                process.DocumentId = documentid.ToString();

                await _processRepository.Create(process);
            }

            return true;
        }

        public bool UpdatePaymentProcess(SignatureStatus value)
        {
            ProcessSearchFilter filter = new ProcessSearchFilter
            {
                UserName = value.User,
                DocumentId = value.DocumentId
            };

            List<Process> processes = _processRepository.Find(filter);
            if (processes.Count == 0) throw new ServiceException("Process not found.", HttpStatusCode.NotFound);
            if (processes.Count > 1) throw new ServiceException("More than one process was found", HttpStatusCode.BadRequest);

            Process process = processes[0];
            if (value.Status == "document_completed") process.ProcessStatus = (int)ProcessStatuses.Accepted;
            else if (value.Status == "document_canceled") process.ProcessStatus = (int)ProcessStatuses.Canceled;
            else throw new ServiceException("Document status not valid", HttpStatusCode.BadRequest);

            _processRepository.Update(process);

            return true;
        }

        private MultipartFormDataContent FillFormBankMethod(Store store, PaymentMethodBank bankmethod, User user)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(user.Name), "recipients[0][name]");
            form.Add(new StringContent(user.Email), "recipients[0][email]");
            form.Add(new StringContent(((int)DocumentTypes.SEPA).ToString()), "documentinformation[0][documenttype]");
            form.Add(new StringContent(store.StoreName), "storeidentification");
            form.Add(new StringContent(SystemTypes.CustomerPortal.ToString()), "sourcesystem");
            form.Add(new StringContent(user.Username), "sourceuser");

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
    }
}
