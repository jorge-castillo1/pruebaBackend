using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class PaymentServices : IPaymentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProcessRepository _processRepository;
        public PaymentServices(IUserRepository userRepository, IProcessRepository processRepository)
        {
            _userRepository = userRepository;
            _processRepository = processRepository;
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
                List<Process> processes =_processRepository.Find(searchProcess);
                if (processes.Count > 0)
                    throw new ServiceException("User have same pending process for this contract number", HttpStatusCode.BadRequest, "ContractNumber", "Pending process");

                //4. Generate and send Document To SignatureAPI
                Guid documentid = Guid.NewGuid(); //From signature API

                //5. Create a change method payment process
                Process process = new Process();
                process.Username = user.Username;
                process.ProcessType = (int)ProcessTypes.PaymentMethodChangeBank;
                process.ProcessStatus = (int)ProcessStatuses.Pending;
                process.DocumentId = documentid.ToString();

                await _processRepository.Create(process);
            }

            return true;
        }
    }
}
