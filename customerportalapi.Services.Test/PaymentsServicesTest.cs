﻿using customerportalapi.Entities;
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
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }

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
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);

            //Assert
            Assert.IsTrue(result);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
            _processRepository.Verify(x => x.Create(It.IsAny<Process>()));
            //Más adelante habrá que verificar que se ha enviado el documento a firmar

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlCambiarElMetodoDePagoPorDomiciliacionSinContrato_SeDevuelveUnaExcepcion()
        {
            //Arrange
            PaymentMethodBank bankdata = new PaymentMethodBank();
            bankdata.Dni = "fake dni";
            bankdata.AccountType = "fake account type";

            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object);
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
            PaymentServices service = new PaymentServices(_userRepository.Object, _processRepository.Object);
            bool result = await service.ChangePaymentMethod(bankdata);
        }
    }
}
