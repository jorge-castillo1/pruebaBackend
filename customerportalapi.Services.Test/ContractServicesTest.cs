using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class ContractServicesTest
    {
        private IConfiguration _configuration;
        private Mock<IUserRepository> _userRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IDistributedCache> _distributedCache;
        private Mock<IIdentityRepository> _identityRepository;
        private Mock<IContractSMRepository> _contractSMRepository;
        private Mock<IMailRepository> _mailRepository;
        private Mock<IEmailTemplateRepository> _emailTemplateRepository;
        private Mock<IDocumentRepository> _documentRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _distributedCache = new Mock<IDistributedCache>();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailTemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _documentRepository = DocumentRepositoryMock.DocumentRepository();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configuration = builder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task WhenDontHaveContractNumberShouldProduceAException()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractInvalid = ContractRepositoryMock.InvalidContractRepository();

            //Act
            ContractServices service = new ContractServices(_configuration, contractInvalid.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object, _documentRepository.Object, _userRepository.Object, _storeRepository.Object);
            await service.GetContractAsync(contractNumber);

        }

        [TestMethod]
        public async Task ShouldReturnAContract()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepository();

            //Act
            ContractServices service = new ContractServices(_configuration, contractRep.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object, _documentRepository.Object, _userRepository.Object, _storeRepository.Object);
            Contract contract = await service.GetContractAsync(contractNumber);

            //Assert
            Assert.IsNotNull(contract);
        }
    }
}
