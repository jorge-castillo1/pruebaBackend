using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class PaymentsServicesTest
    {
        private IConfiguration _configuration;
        private Mock<IUserRepository> _userRepository;
        private Mock<IProcessRepository> _processRepository;
        private Mock<ISignatureRepository> _signatureRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IProfileRepository> _profileRepository;
        private Mock<IAccountSMRepository> _accountSMRepository;
        private Mock<IEmailTemplateRepository> _emailTemplateRepository;
        private Mock<IMailRepository> _mailRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IDocumentRepository> _documentRepository;
        private Mock<ICardRepository> _cardRepository;
        private Mock<IContractSMRepository> _smContractRepository;
        private Mock<IPaymentRepository> _paymentRepository;
        private Mock<IPaymentMethodRepository> _paymentMethodRepository;
        private Mock<IPayRepository> _payRepository;
        Mock<ILogger<PaymentServices>> _logger;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _processRepository = ProcessRepositoryMock.ProcessRepository();
            _signatureRepository = SignatureRepositoryMock.SignatureRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _accountSMRepository = AccountSMRepositoryMock.AccountSMRepository();
            _emailTemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _documentRepository = DocumentRepositoryMock.DocumentRepository();
            _cardRepository = CardRepositoryMock.CardRepository();
            _smContractRepository = ContractSMRepositoryMock.ContractSMRepository();
            _paymentRepository = PaymentRepositoryMock.PaymentRepository();
            _paymentMethodRepository = PaymentMethodRepositoryMock.PaymentMethodRepository();
            _payRepository = PayRepositoryMock.PayRepository();
            _logger = new Mock<ILogger<PaymentServices>>();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configuration = builder.Build();

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoYNoExisteUsuario_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";

            //Act
            _userRepository = UserRepositoryMock.InvalidUserRepository();
            _profileRepository = ProfileRepositoryMock.AccountProfileRepository();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }

        [TestMethod]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacion_SeCreaUnProceso()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank
            {
                Dni = "fake dni",
                AccountType = "fake account type",
                ContractNumber = "fake contract number",
                SmContractCode = "fake sm contract code",
                IBAN = "fake iban",
                FullName = "fake fullname",
                Address = "fake address",
                PostalCode = "fake postal code",
                Location = "fake location",
                State = "fake state",
                Country = "fake country",
                ApsReference = "fake Aps",
                StoreCode = "fake Store Code"
            };

            //Act
            _processRepository = ProcessRepositoryMock.NoPendingSameProcessRepository();
            _profileRepository = ProfileRepositoryMock.AccountProfileRepository();
            _smContractRepository = ContractSMRepositoryMock.UpdateAps();

            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            bool result = await service.ChangePaymentMethod(bankdata);

            //Assert
            Assert.IsTrue(result);

            _userRepository.Verify(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>()));
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
            //_smContractRepository.Verify(x => x.UpdateAps(It.IsAny<ApsRequest>()));
            _storeRepository.Verify(x => x.GetStoreAsync(It.IsAny<string>()));
            _signatureRepository.Verify(x => x.CreateSignature(It.IsAny<MultipartFormDataContent>()));
            //_profileRepository.Verify(x => x.GetAccountByDocumentNumberAsync(It.IsAny<string>()));
            _profileRepository.Verify(x => x.UpdateAccountAsync(It.IsAny<AccountProfile>()));
            _processRepository.Verify(x => x.Create(It.IsAny<Process>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacionSinContrato_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";

            _profileRepository = ProfileRepositoryMock.AccountProfileRepository();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacion_AccountProfileSin_SmContractID_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";

            _profileRepository = ProfileRepositoryMock.AccountProfileRepositoryInvalid();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);

            bool result = await service.ChangePaymentMethod(bankdata);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoDelMismoContrato_ConUnCambioPendiente_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";
            bankdata.ContractNumber = "fake contract number";

            _processRepository = ProcessRepositoryMock.PendingSameProcessRepository();
            _profileRepository = ProfileRepositoryMock.AccountProfileRepository();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }

        /*[TestMethod]
        public void SeModificaUnProceso()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_canceled";

            _processRepository = ProcessRepositoryMock.OneResultProcessRepository();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            var result = service.UpdatePaymentProcess(value);

            Assert.IsNotNull(result);
        }*/

        [TestMethod]
        public void getCardCorrect()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_canceled";

            _cardRepository = CardRepositoryMock.CardRepository();
            PaymentServices service = new PaymentServices(_configuration, _userRepository.Object,
                _processRepository.Object, _signatureRepository.Object, _storeRepository.Object,
                _accountSMRepository.Object, _emailTemplateRepository.Object, _mailRepository.Object,
                _profileRepository.Object, _contractRepository.Object, _paymentRepository.Object,
                _smContractRepository.Object, _cardRepository.Object, _paymentMethodRepository.Object,
                _payRepository.Object, _logger.Object);
            var result = service.GetCard("username", "smContractCode");

            Assert.IsNotNull(result);
        }
    }
}
