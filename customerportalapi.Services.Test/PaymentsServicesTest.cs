using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
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
    public class PaymentsServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IProcessRepository> _processRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _processRepository = ProcessRepositoryMock.ProcessRepository();
        }

        //[TestMethod]
        //public async Task<Process> AlCambiarElMetodoDePagoPorDomiciliacion_SeDevuelveUnProcesoEnEstadoPendiente()
        //{
        //    //Arrange
        //    PaymentMethodBank bankdata = new PaymentMethodBank();
        //    bankdata.Dni = "fake dni";
        //    bankdata.AccountType = "fake account type";
        //    bankdata.ContractNumber = "fake contract number";
        //    bankdata.IBAN = "fake iban";
        //    bankdata.FullName = "fake fullname";
        //    bankdata.Address = "fake address";
        //    bankdata.PostalCode = "fake postal code";
        //    bankdata.Location = "fake location";
        //    bankdata.State = "fake state";
        //    bankdata.Country = "fake country";
            
        //    //Act
        //    PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object);
        //    Process entity = await service.ChangePaymentMethod(bankdata);

        //    //Assert
        //    Assert.IsTrue(entity.Id != string.Empty);
        //    Assert.IsTrue(entity.CreationDate != DateTime.MinValue);
        //    Assert.IsTrue(entity.ProcessStatus == (int)ProcessStatuses.Pending);

        //    //Más adelante habrá que verificar que se ha enviado el documento a firmar

        //}

        //[TestMethod]
        //public async Task<Process> AlCambiarElMetodoDePagoDelMismoContrato_ConUnCambioPendiente_SeDevuelveUnaExcepcion()
        //{
        //}
    }
}
