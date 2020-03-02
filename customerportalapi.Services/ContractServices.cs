using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using System.Net;
using customerportalapi.Services.Exceptions;

namespace customerportalapi.Services
{
    public class ContractServices : IContractServices
    {
        private readonly IContractRepository _contractRepository;
        private readonly IContractSMRepository _contractSMRepository;

        public ContractServices(IContractRepository contractRepository, IContractSMRepository contractSMRepository)
        {
            _contractRepository = contractRepository;
            _contractSMRepository = contractSMRepository;
        }

        public async Task<Contract> GetContractAsync(string contractNumber)
        {
            Contract contract = await _contractRepository.GetContractAsync(contractNumber);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");

            return contract;
        }

        public async Task<ContractFull> GetFullContractAsync(string contractNumber)
        {
            ContractFull response = new ContractFull();
            response.contract = await _contractRepository.GetContractAsync(contractNumber);
            if (response.contract.ContractNumber == null) throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");
            response.smcontract = await _contractSMRepository.GetAccessCodeAsync(contractNumber);
            response.contract.StoreCode = response.contract.StoreData.StoreCode;
            return response;
        }

        public async Task<string> GetDownloadContractAsync(string contractNumber)
        {
            string contractFile = await _contractRepository.GetDownloadContractAsync(contractNumber);
            if (contractFile == "")
                throw new ServiceException("Contract file does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");

            return contractFile;
        }

        public async Task<string> GetContractTimeZoneAsync(string contractNumber)
        {
            var smcontract = await _contractSMRepository.GetAccessCodeAsync(contractNumber);
            return smcontract.Timezone;
        }
    }
}
