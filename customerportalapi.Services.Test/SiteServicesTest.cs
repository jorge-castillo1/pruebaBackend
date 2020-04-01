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
using System.Linq;

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
            string username = "12345678A";
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            SiteServices service = new SiteServices(userRepositoryInvalid.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            await service.GetContractsAsync(username);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarContratosDeUsuarioExistente_DevuelveListaPorEdificio()
        {
            //Arrange
            string username = "12345678A";

            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);
            List<Site> sites = await service.GetContractsAsync(username);

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
            _contractRepository = ContractRepositoryMock.ValidContractRepository();
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

        [TestMethod]
        public async Task AlSolicitarInformacionDeUltimasFacturas_DevuelveLas3Ultimas()
        {
            //Arrange
            string username = "fake user";
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);

            //Act
            List<SiteInvoices> siteInvoices = await service.GetLastInvoices(username);

            //Assert
            Assert.IsTrue(siteInvoices.Count == 2);
            Assert.IsTrue(siteInvoices[0].Invoices.Count(x => x.SiteID == "RI1BBFRI120920060001") == 3);
        }

        [TestMethod]
        public async Task AlSolicitarInformacionDeUltimasFacturasDeUnClienteSinFacturas_NoDevuelveNinguna()
        {
            //Arrange
            string username = "fake user";
            _contractSMRepository = ContractSMRepositoryMock.ContractSMNoInvoiceRepository();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object);

            //Act
            List<SiteInvoices> siteInvoices = await service.GetLastInvoices(username);

            //Assert
            Assert.IsTrue(siteInvoices.Count == 2);
            Assert.IsTrue(siteInvoices[0].Invoices.Count(x => x.SiteID == "RI1BBFRI120920060001") == 0);
        }
    }
}
