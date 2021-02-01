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
using Org.BouncyCastle.Math.EC.Rfc7748;
using Microsoft.Extensions.Configuration;

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
        private IConfiguration _config;
        private Mock<IMailRepository> _mailRepository;
        private Mock<IEmailTemplateRepository> _emailTemplateRepository;


        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _distributedCache = new Mock<IDistributedCache>();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailTemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            //_config = new Mock<IConfiguration>();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _config = builder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarContratosDeUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string username = "12345678A";
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            SiteServices service = new SiteServices(userRepositoryInvalid.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            await service.GetContractsAsync(username);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarContratosDeUsuarioExistente_DevuelveListaPorEdificio()
        {
            //Arrange
            string username = "12345678A";

            //Act
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
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
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            AccessCode entity = await service.GetAccessCodeAsync("fake contractid", "fake password");

            //Assert
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Password == "fake password");

            _identityRepository.Verify(x => x.Authorize(It.IsAny<Login>()));
            //Verify that attempts are incremented
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
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
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            AccessCode entity = await service.GetAccessCodeAsync("fake contractid", "fake password");
        }

        [TestMethod]
        public async Task CodigoAccesoNoDisponibleParaUsuario_Con5IntentosFallidos_Y_Menos15Minutos()
        {
            //Arrange
            GenericIdentity gi = new GenericIdentity("fake name");
            GenericPrincipal gp = new GenericPrincipal(gi, null);
            System.Threading.Thread.CurrentPrincipal = gp;

            _userRepository = UserRepositoryMock.InvalidUserRepository();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            bool available = await service.IsAccessCodeAvailableAsync();
            Assert.IsFalse(available);
        }

        [TestMethod]
        public async Task CodigoAccesoDisponibleParaUsuario_Con5IntentosFallidos_Y_Mas15Minutos()
        {
            //Arrange
            GenericIdentity gi = new GenericIdentity("fake name");
            GenericPrincipal gp = new GenericPrincipal(gi, null);
            System.Threading.Thread.CurrentPrincipal = gp;

            _userRepository = UserRepositoryMock.ValidUserRepository_With5Attempts();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            bool available = await service.IsAccessCodeAvailableAsync();
            Assert.IsTrue(available);
        }

        [TestMethod]
        public async Task CodigoAccesoDisponibleParaUsuario_ConMenos5Intentos()
        {
            //Arrange
            GenericIdentity gi = new GenericIdentity("fake name");
            GenericPrincipal gp = new GenericPrincipal(gi, null);
            System.Threading.Thread.CurrentPrincipal = gp;

            _userRepository = UserRepositoryMock.ValidUserRepository();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);
            bool available = await service.IsAccessCodeAvailableAsync();
            Assert.IsTrue(available);
        }

        [TestMethod]
        public async Task AlSolicitarInformacionDeUltimasFacturas_DevuelveLas3Ultimas_PorContrato()
        {
            //Arrange
            string username = "fake user";
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);

            //Act
            List<SiteInvoices> siteInvoices = await service.GetLastInvoices(username);

            //Assert
            Assert.IsTrue(siteInvoices.Count == 2);
            Assert.IsTrue(siteInvoices[0].Contracts.Count == 2);

            //Invoices by Site & Contract
            Assert.IsTrue(siteInvoices[0].Contracts[0].Invoices.Count(x => x.SiteID == "RI1BBFRI120920060001") == 3);
            Assert.IsTrue(siteInvoices[0].Contracts[1].Invoices.Count(x => x.SiteID == "RI1BBFRI120920060001") == 2);
        }

        [TestMethod]
        public async Task AlSolicitarInformacionDeUltimasFacturasDeUnClienteSinFacturas_NoDevuelveNinguna()
        {
            //Arrange
            string username = "fake user";
            _contractSMRepository = ContractSMRepositoryMock.ContractSMNoInvoiceRepository();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);

            //Act
            List<SiteInvoices> siteInvoices = await service.GetLastInvoices(username);

            //Assert
            Assert.IsTrue(siteInvoices.Count == 2);
            Assert.IsTrue(siteInvoices[0].Contracts[0].Invoices.Count(x => x.SiteID == "RI1BBFRI120920060001") == 0);
        }

        [TestMethod]
        public async Task AlSolicitarInformacionDeUltimasFacturasDeUnClienteSinFacturas_NoDevuelveNingunaFactura_contractoInactivo()
        {
            //Arrange
            string username = "fake user";
            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepositoryInactiveContractSM();
            SiteServices service = new SiteServices(_userRepository.Object, _contractRepository.Object, _storeRepository.Object, _distributedCache.Object, _identityRepository.Object, _contractSMRepository.Object, _config, _mailRepository.Object, _emailTemplateRepository.Object);

            //Act
            List<SiteInvoices> siteInvoices = await service.GetLastInvoices(username);

            //Assert
            Assert.IsTrue(siteInvoices.Count == 2);
            Assert.IsTrue(siteInvoices[0].Contracts.Count == 0);
        }
    }
}
