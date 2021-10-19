﻿//using customerportalapi.Entities;
//using customerportalapi.Repositories.interfaces;
//using customerportalapi.Services.Exceptions;
//using customerportalapi.Services.Test.FakeData;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Distributed;
//using System.Security.Principal;
//using Microsoft.Extensions.Configuration;

//namespace customerportalapi.Services.Test
//{
//    [TestClass]
//    public class ContractServicesTest
//    {
//        private IConfiguration _configuration;
//        private Mock<IUserRepository> _userRepository;
//        private Mock<IContractRepository> _contractRepository;
//        private Mock<IStoreRepository> _storeRepository;
//        private Mock<IDistributedCache> _distributedCache;
//        private Mock<IIdentityRepository> _identityRepository;
//        private Mock<IContractSMRepository> _contractSMRepository;
//        private Mock<IMailRepository> _mailRepository;
//        private Mock<IEmailTemplateRepository> _emailTemplateRepository;
//        private Mock<IDocumentRepository> _documentRepository;
//        private Mock<IOpportunityCRMRepository> _opportunityCRMRepository;
//        private Mock<IPaymentMethodRepository> _paymentMethodRepository;
      
//        [TestInitialize]
//        public void Setup()
//        {
//            _userRepository = UserRepositoryMock.ValidUserRepository();
//            _contractRepository = ContractRepositoryMock.ContractRepository();
//            _storeRepository = StoreRepositoryMock.StoreRepository();
//            _distributedCache = new Mock<IDistributedCache>();
//            _identityRepository = IdentityRepositoryMock.IdentityRepository();
//            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepository();
//            _mailRepository = MailRepositoryMock.MailRepository();
//            _emailTemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
//            _documentRepository = DocumentRepositoryMock.DocumentRepository();
//            _opportunityCRMRepository = OpportunityCRMRepositoryMock.OpportunityCRMRepository();
//            _paymentMethodRepository = PaymentMethodRepositoryMock.PaymentMethodRepository();

//        var builder = new ConfigurationBuilder();
//            builder.AddJsonFile("appsettings.json");
//            _configuration = builder.Build();
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenDontHaveContractNumberShouldProduceAException()
//        {
//            //Arrange
//            string contractNumber = "TRWETR436745732564536";
//            Mock<IContractRepository> contractInvalid = ContractRepositoryMock.InvalidContractRepository();

//            //Act
//s

//        [TestMethod]
//        public async Task ShouldReturnAContract()
//        {
//            //Arrange
//            string contractNumber = "TRWETR436745732564536";
//            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, 
//                contractRep.Object, 
//                _contractSMRepository.Object, 
//                _mailRepository.Object, 
//                _emailTemplateRepository.Object, 
//                _documentRepository.Object, 
//                _userRepository.Object, 
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            Contract contract = await service.GetContractAsync(contractNumber);

//            //Assert
//            Assert.IsNotNull(contract);
//        }

//        [TestMethod]
//        public async Task ShouldReturnAContractFile()
//        {
//            //Arrange
//            string dni = "dni";
//            string smContractCode = "R1234567890";
//            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration,
//                contractRep.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                _documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadContractAsync(dni, smContractCode);

//            //Assert
//            Assert.IsNotNull(document);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadContract_And_FindMultipleDocs_ShouldProduceAException()
//        {
//            //Arrange
//            string dni = "dni";
//            string smContractCode = "R1234567890";
//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.MultipleContractsRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadContractAsync(dni, smContractCode);

//            //Assert

//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadContract_And_DontFindFindEmailTemplate_ShouldProduceAException()
//        {
//            //Arrange
//            string dni = "dni";
//            string smContractCode = "R1234567890";

//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();
//            Mock<IEmailTemplateRepository> emailTemplateRepository = EmailTemplateRepositoryMock.Invalid_EmailTemplateRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadContractAsync(dni, smContractCode);

//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadContract_And_DontFindContractStoreEmail_ShouldProduceAException()
//        {
//            //Arrange
//            string dni = "dni";
//            string smContractCode = "R1234567890";
//            Mock<IContractRepository> contractRepository = ContractRepositoryMock.ValidContractRepositoryWithoutStoreEmail();
//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadContractAsync(dni, smContractCode);

//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadContract_And_DontFindDocument_ShouldProduceAException()
//        {
//            //Arrange
//            string dni = "dni";
//            string smContractCode = "R1234567890";
//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadContractAsync(dni, smContractCode);

//            //Assert
//        }

         
//        [TestMethod]
//        public async Task ShouldReturnAInvoiceFile()
//        {
//            //Arrange
//            InvoiceDownload invoice = new InvoiceDownload()
//            {
//                InvoiceNumber = "FakeInvoiceNumber",
//                StoreCode = "FakeStoreCode",
//                Username = "FakeUsername"
//            };

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration,
//                _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                _documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadInvoiceAsync(invoice);

//            //Assert
//            Assert.IsNotNull(document);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadInvoice_And_FindMultipleInvoiceDocs_ShouldProduceAException()
//        {
//            //Arrange
//            InvoiceDownload invoice = new InvoiceDownload()
//            {
//                InvoiceNumber = "FakeInvoiceNumber",
//                StoreCode = "FakeStoreCode",
//                Username = "FakeUsername"
//            };
//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.MultipleInvoiceRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadInvoiceAsync(invoice);

//            //Assert

//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadInvoice_And_DontFindFindEmailTemplate_ShouldProduceAException()
//        {
//            //Arrange
//            InvoiceDownload invoice = new InvoiceDownload()
//            {
//                InvoiceNumber = "FakeInvoiceNumber",
//                StoreCode = "FakeStoreCode",
//                Username = "FakeUsername"
//            };

//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();
//            Mock<IEmailTemplateRepository> emailTemplateRepository = EmailTemplateRepositoryMock.Invalid_EmailTemplateRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadInvoiceAsync(invoice);

//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadInvoice_And_DontFindContractStoreEmail_ShouldProduceAException()
//        {
//            //Arrange
//            InvoiceDownload invoice = new InvoiceDownload()
//            {
//                InvoiceNumber = "FakeInvoiceNumber",
//                StoreCode = "FakeStoreCode",
//                Username = "FakeUsername"
//            };
//            Mock<IStoreRepository> storeRepository = StoreRepositoryMock.StoreRepositoryWithoutEmail();
//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadInvoiceAsync(invoice);

//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetDownloadInvoice_And_DontFindDocument_ShouldProduceAException()
//        {
//            //Arrange
//            InvoiceDownload invoice = new InvoiceDownload()
//            {
//                InvoiceNumber = "FakeInvoiceNumber",
//                StoreCode = "FakeStoreCode",
//                Username = "FakeUsername"
//            };

//            Mock<IDocumentRepository> documentRepository = DocumentRepositoryMock.NoContractnumberDocumentRepository();


//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, _contractRepository.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            string document = await service.GetDownloadInvoiceAsync(invoice);

//            //Assert
//        }

//        [TestMethod]
//        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
//        public async Task WhenGetcontractFull_And_DontHaveContractNumber_ShouldProduceAException()
//        {
//            //Arrange
//            string smContractCode = "R1234567890";
//            Mock<IContractRepository> contractInvalid = ContractRepositoryMock.InvalidContractRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration, contractInvalid.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                _documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            await service.GetFullContractAsync(smContractCode);

//        }

//        [TestMethod]
//        public async Task ShouldReturnAFullContract_Without_PaymentMethodDescription()
//        {
//            //Arrange
//            string smContractCode = "R1234567890";
//            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepository();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration,
//                contractRep.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                _documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            ContractFull contractFull = await service.GetFullContractAsync(smContractCode);

//            //Assert
//            Assert.AreEqual(contractFull.contract.SmContractCode, smContractCode);
//            Assert.IsNotNull(contractFull.smcontract);
//            Assert.IsNull(contractFull.contract.PaymentMethodDescription);
//        }

//        [TestMethod]
//        public async Task ShouldReturnAFullContract_With_PaymentMethodDescription()
//        {
//            //Arrange
//            string smContractCode = "R1234567890";
//            Mock<IContractRepository> contractRep = ContractRepositoryMock.ValidContractRepositoryWithPaymentMethod();

//            //Act
//            ContractServices service = new ContractServices
//            (
//                _configuration,
//                contractRep.Object,
//                _contractSMRepository.Object,
//                _mailRepository.Object,
//                _emailTemplateRepository.Object,
//                _documentRepository.Object,
//                _userRepository.Object,
//                _storeRepository.Object,
//                _opportunityCRMRepository.Object,
//                _paymentMethodRepository.Object
//            );
//            ContractFull contractFull = await service.GetFullContractAsync(smContractCode);

//            //Assert
//            Assert.AreEqual(contractFull.contract.SmContractCode, smContractCode);
//            Assert.IsNotNull(contractFull.smcontract);
//            Assert.IsNotNull(contractFull.contract.PaymentMethodDescription);
//        }
//    }
//}
