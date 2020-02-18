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
    public class SiteServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IDistributedCache> _distributedCache;
        private Mock<IIdentityRepository> _identityRepository;
        private Mock<IContractSMRepository> _contractSMRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _distributedCache = new Mock<IDistributedCache>();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepository();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarContratosDeUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            SiteServices service = new SiteServices(userRepositoryInvalid.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            await service.GetContractsAsync(dni, AccountType.Residential);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarContratosDeUsuarioExistente_DevuelveListaPorEdificio()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            List<Site> sites = await service.GetContractsAsync(dni, AccountType.Residential);

            //Assert
            Assert.IsNotNull(sites);
            Assert.IsTrue(sites.Count >= 1);
            foreach(Site s in sites)
            {
                Assert.IsTrue(s.Contracts.Count >= 1);
            }
        }

        [TestMethod]
        public async Task AlSolicitarCodigoAcceso_DevuelveCodigoYTokenvalido()
        {
            //Arrange
            GenericIdentity gi = new GenericIdentity("fake name");
            GenericPrincipal gp = new GenericPrincipal(gi, null);
            System.Threading.Thread.CurrentPrincipal = gp;
            
            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            AccessCode entity = await service.GetAccessCodeAsync("fake contractid", "fake password");

            //Assert
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Password == "fake password");

            _identityRepository.Verify(x => x.Authorize(It.IsAny<Login>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlSolicitarCodigoAccesoConUsuarioInvalido_SeProduceUnaExcepcion()
        {
            //Arrange
            GenericIdentity gi = new GenericIdentity("fake name");
            GenericPrincipal gp = new GenericPrincipal(gi, null);
            System.Threading.Thread.CurrentPrincipal = gp;

            //Act
            _identityRepository = IdentityRepositoryMock.IdentityRepository_Invalid();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            AccessCode entity = await service.GetAccessCodeAsync("fake contractid", "fake password");

            //Assert
        }
    }
}
