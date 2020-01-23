using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class SiteServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IDistributedCache> _distributedCache;
        

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _distributedCache = new Mock<IDistributedCache>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarContratosDeUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            SiteServices service = new SiteServices(userRepositoryInvalid.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object);
            await service.GetContractsAsync(dni);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarContratosDeUsuarioExistente_DevuelveListaPorEdificio()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object);
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
