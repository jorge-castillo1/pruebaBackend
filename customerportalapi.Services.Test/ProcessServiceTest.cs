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

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class ProcessServiceTest
    {
        private Mock<IProcessRepository> _processRepository;
        private Mock<ISignatureRepository> _signatureRepository;
        private Mock<IPaymentRepository> _paymentRepository;

        [TestInitialize]
        public void Setup()
        {
            _processRepository = ProcessRepositoryMock.ProcessRepository();
            _signatureRepository = SignatureRepositoryMock.SignatureRepository();
            _paymentRepository = PaymentRepositoryMock.PaymentRepository();
        }

        [TestMethod]
        public void SiSeEncuentranProcesos_DevuelveElMasReciente()
        {
            //Arrange
            string user = "fake user";
            string contractnumber = "fake contract";
            int processtype = 1;

            _processRepository = ProcessRepositoryMock.MoreThanOneResultProcessRepository();
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object, _paymentRepository.Object);
            var result = service.GetLastProcesses(user, contractnumber, processtype);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result[0].ProcessStatus);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
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
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object, _paymentRepository.Object);
            var result = service.UpdateSignatureProcess(value);

            Assert.IsNotNull(result);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
            _processRepository.Verify(x => x.Update(It.IsAny<Process>()));
        }

        [TestMethod]
        public void SiNoExisteRegistro_ConElMismoUsuarioYDocumento_SeDevuelveNull()
        {
            //Arrange
            SignatureStatus value = new SignatureStatus();
            value.User = "usertest";
            value.DocumentId = Guid.NewGuid().ToString();
            value.Status = "document_completed";

            _processRepository = ProcessRepositoryMock.NoResultsProcessRepository();
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object, _paymentRepository.Object);
            Process process = service.UpdateSignatureProcess(value);
            Assert.IsNull(process);
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
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object, _paymentRepository.Object);
            service.UpdateSignatureProcess(value);
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
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object, _paymentRepository.Object);
            service.UpdateSignatureProcess(value);
        }
    }
}
