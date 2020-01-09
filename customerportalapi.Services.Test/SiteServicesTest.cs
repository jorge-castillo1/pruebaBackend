using customerportalapi.Entities;
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
    public class SiteServicesTest
    {
        Mock<IUserRepository> _userRepository;
        Mock<IContractRepository> _contractRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarContratosDeUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            SiteServices service = new SiteServices(_userRepositoryInvalid.Object, _contractRepository.Object);
            await service.GetContractsAsync(dni);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarContratosDeUsuarioExistente_DevuelveListaPorEdificio()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object);
            List<Site> sites = await service.GetContractsAsync(dni);

            //Assert
            Assert.IsNotNull(sites);
            Assert.IsTrue(sites.Count >= 1);
            foreach(Site s in sites)
            {
                Assert.IsTrue(s.Contracts.Count >= 1);
            }
        }
    }
}
