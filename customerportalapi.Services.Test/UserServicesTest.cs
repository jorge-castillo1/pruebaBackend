using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Repositories;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class UserServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IProfileRepository> _profileRepository;
        private Mock<IMailRepository> _mailRepository;
        private Mock<IEmailTemplateRepository> _emailtemplateRepository;
        private Mock<IIdentityRepository> _identityRepository;
        private IConfigurationRoot _config;
        private ILoginService _serviceLogin;
        private Mock<IUserAccountRepository> _userAccountRepository;
        private Mock<ILanguageRepository> _languageRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IContractSMRepository> _contractSmRepository;
        private Mock<IOpportunityCRMRepository> _opportunityRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IUnitLocationRepository> _unitLocationRepository;
        private Mock<IFeatureRepository> _featureRepository;
        private Mock<INewUserRepository> _newUserRepository;
        private Mock<IGoogleCaptchaRepository> _googleCaptchaRepository;
        Mock<ILogger<UserServices>> _logger;


        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");

            _userRepository = UserRepositoryMock.ValidUserRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailtemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _config = builder.Build();
            _serviceLogin = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            _userAccountRepository = UserAccountRepositoryMock.ValidUserRepository();
            _languageRepository = LanguageRepositoryMock.LanguageRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _contractSmRepository = ContractSMRepositoryMock.ContractSMRepository();
            _opportunityRepository = OpportunityCRMRepositoryMock.OpportunityCRMRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _unitLocationRepository = UnitLocationRepositoryMock.UnitLocationRepository();
            _featureRepository = FeatureRepositoryMock.FeatureRepository();
            _newUserRepository = NewUserRepositoryMock.ValidNewUserRepository();
            _googleCaptchaRepository = GoogleCaptchaRepositoryMock.GoogleCaptchaRepository();
            _logger = new Mock<ILogger<UserServices>>();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlSolicitarUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            var username = "12345678A";
            var userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config, _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                 _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
            );
            await service.GetProfileAsync(username);
            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarUnUsuarioExistente_DevuelvePerfil()
        {
            //Arrange
            var username = "12345678A";

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var usuario = await service.GetProfileAsync(username);

            //Assert
            Assert.IsNotNull(usuario);
            Assert.AreEqual("fake profile image", usuario.Avatar);
            Assert.AreEqual("fake name", usuario.Fullname);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlActualizarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            var profile = new Profile { DocumentNumber = "12345678A" };
            var userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlActualizarUnUsuarioExistente_SinEmails_SeProducenErrores()
        {
            //Arrange
            var profile = new Profile
            {
                DocumentNumber = "12345678A",
                Language = "new language",
                EmailAddress1 = string.Empty,
                EmailAddress1Principal = false,
                EmailAddress2 = string.Empty,
                EmailAddress2Principal = false,
                Avatar = "new profile image"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlActualizarUnUsuarioExistente_ConEmailPrincipal1Invalido_SeProducenErrores()
        {
            //Arrange
            var profile = new Profile
            {
                DocumentNumber = "12345678A",
                Language = "new language",
                EmailAddress1 = string.Empty,
                EmailAddress1Principal = true,
                EmailAddress2 = "fake email 2",
                EmailAddress2Principal = false,
                Avatar = "new profile image"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        public async Task AlActualizarUnUsuarioExistente_NoSeProducenErrores()
        {
            //Arrange
            var profile = new Profile
            {
                DocumentNumber = "12345678A",
                Language = "new language",
                EmailAddress1 = "fake email 1 modified",
                EmailAddress1Principal = true,
                EmailAddress2 = "fake email 2",
                EmailAddress2Principal = false,
                Avatar = "new profile image"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.UpdateProfileAsync(profile);

            //Assert
            _userRepository.Verify(x => x.GetCurrentUserByEmail(It.IsAny<string>()));
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
            _profileRepository.Verify(x => x.UpdateProfileAsync(It.IsAny<Profile>()));

            Assert.AreEqual("fake Address modified", result.Address);
            Assert.AreEqual("en", result.Language);
            Assert.AreEqual("fake email 1 modified", result.EmailAddress1);
            Assert.IsTrue(result.EmailAddress1Principal);
            Assert.IsFalse(result.EmailAddress2Principal);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioSinDni_DevuelveExcepcion()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = string.Empty,
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.InviteUserAsync(invitation);

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioSinEmail_DevuelveExcepcion()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = string.Empty,
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.InviteUserAsync(invitation);
        }

        [TestMethod]
        public async Task AlInvitarUnUsuarioNoExistente_SeCreaUnNuevoUsuario()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>(), It.IsAny<bool>()));
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.WelcomeEmailExtended, It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioExistente_Activo_DevuelveError()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.InviteUserAsync(invitation);

            //Assert
        }

        [TestMethod]
        public async Task AlInvitarUnUsuarioExistente_NoActivo_ActualizaUsuario()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUser_Repository();

            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            userRepositoryInvalid.Verify(x => x.Update(It.IsAny<User>()));
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.WelcomeEmailShort, It.IsAny<string>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>(), It.IsAny<bool>()));
        }

        [TestMethod]
        public async Task AlInvitarUnUsuario_AunqueNoExistaPlantillaCorreoEnSuIdioma_SeEnviaCorreoIgualmente()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "Portuguese",
                InvokedBy = 0
            };

            //Act
            var userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>(), It.IsAny<bool>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioConUnEmailEnUso_RetornaError()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var userRepositoryInvalid = UserRepositoryMock.ValidUserRepository_ByEmail();
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            userRepositoryInvalid.Verify(x => x.GetCurrentUserByEmail(It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuario_HacenFaltaDatos_paraCompletarLaInvitacion_EnviaEmailErroraITBlue_RetornaError()
        {
            //Arrange
            var invitation = new Invitation
            {
                Dni = "FakeDni",
                Email = "fakeuser@email.com",
                CustomerType = "Residential",
                Fullname = "Fake User",
                Language = "French"
            };

            //Act
            var userRepository = UserRepositoryMock.InvalidUserRepository();
            var contractRepository = ContractRepositoryMock.InvalidContractRepository();
            var service = new UserServices(
                userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.InvitationError, It.IsAny<string>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>(), It.IsAny<bool>()));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlConfirmarUnUsuarioSinToken_DevuelveExcepcion()
        {
            //Arrange
            var invitationToken = string.Empty;

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.ConfirmUserAsync(invitationToken);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_Activo_DevuelveTokenVacio()
        {
            //Arrange
            var invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            var userRepositoryInvalid = UserRepositoryMock.Invalid_ActiveUserByToken_Repository();
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var tokenResult = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.IsNull(tokenResult.AccesToken);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_NoActivo_DevuelveToken()
        {
            //Arrange
            var invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            var userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUserByToken_Repository();
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var tokenResult = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.AreEqual("Fake AccessToken", tokenResult.AccesToken);
            _identityRepository.Verify(x => x.AddUser(It.IsAny<UserIdentity>()));
            _profileRepository.Verify(x => x.GetProfileAsync(It.IsAny<string>(), It.IsAny<string>()));
            _profileRepository.Verify(x => x.UpdateProfileAsync(It.IsAny<Profile>()));
            _identityRepository.Verify(x => x.FindGroup(It.IsAny<string>()));
            _identityRepository.Verify(x => x.AddUserToGroup(It.IsAny<UserIdentity>(), It.IsAny<Group>()));
            userRepositoryInvalid.Verify(x => x.Update(It.IsAny<User>()));
            _profileRepository.Verify(x => x.ConfirmedWebPortalAccessAsync(It.IsAny<string>(), It.IsAny<string>()));
            _identityRepository.Verify(x => x.Authorize(It.IsAny<Login>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlDesinvitarUnUsuarioSinDni_DevuelveExcepcion()
        {
            //Arrange
            var value = new Invitation()
            {
                Dni = string.Empty,
                CustomerType = AccountType.Business
            };

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.UnInviteUserAsync(value);

            //_identityRepository.Verify(x => x.DeleteUser(string userId));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlDesinvitarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            var value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Business
            };
            var userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            var service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            await service.UnInviteUserAsync(value);
        }


        [TestMethod]
        public async Task AlDesinvitarUnUsuarioExistente_ConExternalId_retorna_true()
        {
            //Arrange
            var value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Residential
            };
            var userRepository = UserRepositoryMock.ValidUserRepository_With_ExternalId();

            //Act
            var service = new UserServices(
                userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.UnInviteUserAsync(value);

            //Assert
            Assert.IsTrue(result);
            userRepository.Verify(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>()));
            _profileRepository.Verify(x => x.RevokedWebPortalAccessAsync(It.IsAny<string>(), It.IsAny<string>()));
            _identityRepository.Verify(x => x.DeleteUser(It.IsAny<string>()));
            userRepository.Verify(x => x.Delete(It.IsAny<User>()));
        }

        [TestMethod]
        public async Task AlDesinvitarUnUsuarioExistente_SinExternalid_retorna_true()
        {
            //Arrange
            var value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Residential
            };
            var userRepository = UserRepositoryMock.ValidUserRepository();

            //Act
            var service = new UserServices(
                userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );
            var result = await service.UnInviteUserAsync(value);

            //Assert
            Assert.IsTrue(result);
            userRepository.Verify(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>()));
            _profileRepository.Verify(x => x.RevokedWebPortalAccessAsync(It.IsAny<string>(), It.IsAny<string>()));
            userRepository.Verify(x => x.Delete(It.IsAny<User>()));
        }


        [TestMethod]
        public async Task AlbuscarCountrydelContrato_segunTablaFeatures_retornaPlantillaWelcome()
        {
            //Arrange
            var contract = new Contract()
            {
                ContractId = "",
                StoreCode = "FR",
                StoreData = new Store(){
                    CountryCode = "FR",
                }
            };
            List<Contract> listContract = new List<Contract>();
            listContract.Add(contract);
            var contractRepository = ContractRepositoryMock.ContractRepositoryFeature();
            var feat = MongoFeaturesRepositoryMock.FeatureRepository_WelcomeLong();
            var featureRepository = new FeatureRepository(null, feat.Object);

            //Act
            var service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                contractRepository.Object,
                _contractSmRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                featureRepository,
                _newUserRepository.Object,
                _googleCaptchaRepository.Object,
                _logger.Object
                );


            var result = await service.GetWelcomeTemplateFromFeatures( listContract, true, (int)InviteInvocationType.CRM);


            //Assert
            Assert.AreEqual(result, 0);

        }
    }
}