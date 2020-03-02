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

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class ContractServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IDistributedCache> _distributedCache;
        private Mock<IIdentityRepository> _identityRepository;
        private Mock<IContractSMRepository> _contractSMRepository;
        private Mock<IMailRepository> _mailRepository;
        private Mock<IEmailTemplateRepository> _emailTemplateRepository;

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
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task WhenDontHaveContractNumberShouldProduceAException()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractInvalid = ContractRepositoryMock.InvalidContractRepository();

            //Act
            ContractServices service = new ContractServices(contractInvalid.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object);
            await service.GetContractAsync(contractNumber);

        }

        [TestMethod]
        public async Task ShouldReturnAContract()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepository();

            //Act
            ContractServices service = new ContractServices(contractRep.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object);
            Contract contract = await service.GetContractAsync(contractNumber);

            //Assert
            Assert.IsNotNull(contract);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task WhenDontHaveContractNumberShouldProduceAExceptionWhenCallGetDownloadContractAsync()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractInvalid = ContractRepositoryMock.InvalidDownloadContractRepository();

            //Act
            ContractServices service = new ContractServices(contractInvalid.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object);
            await service.GetDownloadContractAsync(contractNumber);

        }

        [TestMethod]
        public async Task ShouldReturnAStringWhenCallGetDownloadContractAsync()
        {
            //Arrange
            string contractNumber = "TRWETR436745732564536";
            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidDownloadContractRepository();
            //Act
            ContractServices service = new ContractServices(contractRep.Object, _contractSMRepository.Object, _mailRepository.Object, _emailTemplateRepository.Object);
            string contract = await service.GetDownloadContractAsync(contractNumber);

            //Assert
            Assert.IsNotNull(contract);
        }


    }
}
