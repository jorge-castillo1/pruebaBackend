using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace customerportalapi.Services.Test
{
    [TestClass]
    public class PaymentsServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IProcessRepository> _processRepository;
        private Mock<ISignatureRepository> _signatureRepository;
        private Mock<IStoreRepository> _storeRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _processRepository = ProcessRepositoryMock.ProcessRepository();
            _signatureRepository = SignatureRepositoryMock.SignatureRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
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
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }
        /*
        [TestMethod]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacion_SeCreaUnProceso()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";
            bankdata.ContractNumber = "fake contract number";
            bankdata.IBAN = "fake iban";
            bankdata.FullName = "fake fullname";
            bankdata.Address = "fake address";
            bankdata.PostalCode = "fake postal code";
            bankdata.Location = "fake location";
            bankdata.State = "fake state";
            bankdata.Country = "fake country";

            //Act
            _processRepository = ProcessRepositoryMock.NoPendingSameProcessRepository();
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);

            //Assert
            Assert.IsTrue(result);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
            _processRepository.Verify(x => x.Create(It.IsAny<Process>()));
            _signatureRepository.Verify(x => x.CreateSignature(It.IsAny<MultipartFormDataContent>()));

        }
        */
        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacionSinContrato_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";

            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
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
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public void SiNoExisteRegistro_ConElMismoUsuarioYDocumento_SeDevuelveUnaExcepcion()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_completed";

            _processRepository = ProcessRepositoryMock.NoResultsProcessRepository();
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            var result = service.UpdatePaymentProcess(value);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public void SiExisteMasDeUnRegistro_ConElMismoUsuarioYDocumento_SeDevuelveUnaExcepcion()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_completed";

            _processRepository = ProcessRepositoryMock.MoreThanOneResultProcessRepository();
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            var result = service.UpdatePaymentProcess(value);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public void SiElEstado_NoEsValido_SeDevuelveUnaExcepcion()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "fake_document_state";

            _processRepository = ProcessRepositoryMock.OneResultProcessRepository();
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            var result = service.UpdatePaymentProcess(value);
        }

        [TestMethod]
        public void SeModificaUnProceso()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_canceled";

            _processRepository = ProcessRepositoryMock.OneResultProcessRepository();
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object, _signatureRepository.Object, _storeRepository.Object);
            var result = service.UpdatePaymentProcess(value);

            Assert.IsNotNull(result);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
            _processRepository.Verify(x => x.Update(It.IsAny<Process>()));
        }
    }
}
