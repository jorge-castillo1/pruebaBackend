using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using System.Net;
using customerportalapi.Services.Exceptions;
using Microsoft.Extensions.Configuration;

namespace customerportalapi.Services
{
    public class ContractServices : IContractServices
    {
        private readonly IConfiguration _configuration;
        private readonly IContractRepository _contractRepository;
        private readonly IContractSMRepository _contractSMRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public ContractServices(IConfiguration configuration, IContractRepository contractRepository, IContractSMRepository contractSMRepository, IMailRepository mailRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _configuration = configuration;
            _contractRepository = contractRepository;
            _contractSMRepository = contractSMRepository;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task<Contract> GetContractAsync(string contractNumber)
        {
            Contract contract = await _contractRepository.GetContractAsync(contractNumber);
            if (contract.ContractNumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");

            return contract;
        }


        public async Task<string> GetDownloadContractAsync(string dni, string contractNumber)
        {
            string contractFile = await _contractRepository.GetDownloadContractAsync(contractNumber);
            if (contractFile == "")
            {
                var contract = await GetContractAsync(contractNumber);

                EmailTemplate requestDigitalContractTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.RequestDigitalContract, LanguageTypes.en.ToString()); // TODO: canviar d'idioma
                if (requestDigitalContractTemplate._id != null)
                {
                    Email message = new Email();
                    string mailTo = contract.StoreData.EmailAddress1;
                    if (mailTo == null) throw new ServiceException("Store mail not found", HttpStatusCode.NotFound);
                    if (!(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == _configuration["EnvProduction"])) mailTo = _configuration["MailStores"];
                    message.To.Add(mailTo);
                    message.Subject = string.Format(requestDigitalContractTemplate.subject, contract.Customer, dni);
                    message.Body = string.Format(requestDigitalContractTemplate.body, contract.Customer, dni, contractNumber);
                    await _mailRepository.Send(message);
                }
                throw new ServiceException("Contract file does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");
            }
            if (contractFile == null)
            {
                throw new ServiceException("Something went wrong", HttpStatusCode.BadRequest);
            }

            return contractFile;
        }

        public async Task<ContractFull> GetFullContractAsync(string contractNumber)
        {
            ContractFull response = new ContractFull();
            response.contract = await _contractRepository.GetContractAsync(contractNumber);
            if (response.contract.ContractNumber == null) throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "ContractNumber", "Not exist");
            response.contract.TotalPrice = response.contract.Price + response.contract.Vat;
            String smContractNumber = response.contract.SmContractCode;
            response.smcontract = await _contractSMRepository.GetAccessCodeAsync(smContractNumber);
            response.contract.StoreCode = response.contract.StoreData.StoreCode;
            return response;
        }

        public async Task<string> SaveContractAsync(Document document)
        {
            return await _contractRepository.SaveContractAsync(document);
        }

        public async Task<string> GetContractTimeZoneAsync(string contractNumber)
        {
            var smcontract = await _contractSMRepository.GetAccessCodeAsync(contractNumber);
            return smcontract.Timezone;
        }
    }
}
