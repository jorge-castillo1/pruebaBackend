using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using customerportalapi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;

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
        private Mock<IConfiguration> _config;
        private ILoginService _serviceLogin;
        private Mock<IUserAccountRepository> _userAccountRepository;
        private Mock<ILanguageRepository> _languageRepository;
        private Mock<IContractRepository> _contractRepository;
        private Mock<IContractSMRepository> _contractSMRepository;
        private Mock<IOpportunityCRMRepository> _opportunityRepository;
        private Mock<IStoreRepository> _storeRepository;
        private Mock<IUnitLocationRepository> _unitLocationRepository;
        private Mock<IFeatureRepository> _featureRepository;



        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailtemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _config = new Mock<IConfiguration>();
            _serviceLogin = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config.Object);
            _userAccountRepository = UserAccountRepositoryMock.ValidUserRepository();
            _languageRepository = LanguageRepositoryMock.LanguageRepository();
            _contractRepository = ContractRepositoryMock.ContractRepository();
            _contractSMRepository = ContractSMRepositoryMock.ContractSMRepository();
            _opportunityRepository = OpportunityCRMRepositoryMock.OpportunityCRMRepository();
            _storeRepository = StoreRepositoryMock.StoreRepository();
            _unitLocationRepository = UnitLocationRepositoryMock.UnitLocationRepository();
            _featureRepository = FeatureRepositoryMock.FeatureRepository();

        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlSolicitarUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string username = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object, _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                 _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
            );
            await service.GetProfileAsync(username);
            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarUnUsuarioExistente_DevuelvePerfil()
        {
            //Arrange
            string username = "12345678A";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Profile usuario = await service.GetProfileAsync(username);

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
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlActualizarUnUsuarioExistente_SinEmails_SeProducenErrores()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            profile.Language = "new language";
            profile.EmailAddress1 = string.Empty;
            profile.EmailAddress1Principal = false;
            profile.EmailAddress2 = string.Empty;
            profile.EmailAddress2Principal = false;
            profile.Avatar = "new profile image";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlActualizarUnUsuarioExistente_ConEmailPrincipal1Invalido_SeProducenErrores()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            profile.Language = "new language";
            profile.EmailAddress1 = string.Empty;
            profile.EmailAddress1Principal = true;
            profile.EmailAddress2 = "fake email 2";
            profile.EmailAddress2Principal = false;
            profile.Avatar = "new profile image";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        public async Task AlActualizarUnUsuarioExistente_NoSeProducenErrores()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            profile.Language = "new language";
            profile.EmailAddress1 = "fake email 1 modified";
            profile.EmailAddress1Principal = true;
            profile.EmailAddress2 = "fake email 2";
            profile.EmailAddress2Principal = false;
            profile.Avatar = "new profile image";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Profile result = await service.UpdateProfileAsync(profile);

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
            Invitation invitation = new Invitation();
            invitation.Dni = string.Empty;
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.InviteUserAsync(invitation);

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioSinEmail_DevuelveExcepcion()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = string.Empty;
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.InviteUserAsync(invitation);
        }

        [TestMethod]
        public async Task AlInvitarUnUsuarioNoExistente_SeCreaUnNuevoUsuario()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.WelcomeEmailExtended, It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioExistente_Activo_DevuelveError()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
        }

        [TestMethod]
        public async Task AlInvitarUnUsuarioExistente_NoActivo_ActualizaUsuario()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUser_Repository();

            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Update(It.IsAny<User>()));
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.WelcomeEmailShort, It.IsAny<string>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

        [TestMethod]
        public async Task AlInvitarUnUsuario_AunqueNoExistaPlantillaCorreoEnSuIdioma_SeEnviaCorreoIgualmente()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "Portuguese";

            //Act
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuarioConUnEmailEnUso_RetornaError()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.ValidUserRepository_ByEmail();
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.GetCurrentUserByEmail(It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlInvitarUnUsuario_HacenFaltaDatos_paraCompletarLaInvitacion_EnviaEmailErroraITBlue_RetornaError()
        {
            //Arrange
            Invitation invitation = new Invitation();
            invitation.Dni = "FakeDni";
            invitation.Email = "fakeuser@email.com";
            invitation.CustomerType = "Residential";
            invitation.Fullname = "Fake User";
            invitation.Language = "French";

            //Act
            Mock<IUserRepository> _userRepository = UserRepositoryMock.InvalidUserRepository();
            Mock<IContractRepository> _contractRepository = ContractRepositoryMock.InvalidContractRepository();
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _emailtemplateRepository.Verify(x => x.getTemplate((int)EmailTemplateTypes.InvitationError, It.IsAny<string>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlConfirmarUnUsuarioSinToken_DevuelveExcepcion()
        {
            //Arrange
            string invitationToken = string.Empty;

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.ConfirmUserAsync(invitationToken);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_Activo_DevuelveTokenVacio()
        {
            //Arrange
            string invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.Invalid_ActiveUserByToken_Repository();
            UserServices service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Token tokenResult = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.IsNull(tokenResult.AccesToken);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_NoActivo_DevuelveToken()
        {
            //Arrange
            string invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUserByToken_Repository();
            UserServices service = new UserServices(
                userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            Token tokenResult = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.AreEqual("Fake AccessToken", tokenResult.AccesToken);
            _identityRepository.Verify(x => x.AddUser(It.IsAny<UserIdentity>()));
            _profileRepository.Verify(x => x.GetProfilePermissionsAsync(It.IsAny<string>(), It.IsAny<string>()));
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
            Invitation value = new Invitation()
            {
                Dni = string.Empty,
                CustomerType = AccountType.Business
            };

            //Act
            UserServices service = new UserServices(
                _userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.UnInviteUserAsync(value);

            //_identityRepository.Verify(x => x.DeleteUser(string userId));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepci�n esperada.")]
        public async Task AlDesinvitarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            Invitation value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Business
            };
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(
                _userRepositoryInvalid.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            await service.UnInviteUserAsync(value);
        }


        [TestMethod]
        public async Task AlDesinvitarUnUsuarioExistente_ConExternalId_retorna_true()
        {
            //Arrange
            Invitation value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Residential
            };
            Mock<IUserRepository> userRepository = UserRepositoryMock.ValidUserRepository_With_ExternalId();

            //Act
            UserServices service = new UserServices(
                userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.UnInviteUserAsync(value);

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
            Invitation value = new Invitation()
            {
                Dni = "12345678A",
                CustomerType = AccountType.Residential
            };
            Mock<IUserRepository> userRepository = UserRepositoryMock.ValidUserRepository();

            //Act
            UserServices service = new UserServices(
                userRepository.Object,
                _profileRepository.Object,
                _mailRepository.Object,
                _emailtemplateRepository.Object,
                _identityRepository.Object,
                _config.Object,
                _serviceLogin,
                _userAccountRepository.Object,
                _languageRepository.Object,
                _contractRepository.Object,
                _contractSMRepository.Object,
                _opportunityRepository.Object,
                _storeRepository.Object,
                _unitLocationRepository.Object,
                _featureRepository.Object
                );
            bool result = await service.UnInviteUserAsync(value);

            //Assert
            Assert.IsTrue(result);
            userRepository.Verify(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>()));
            _profileRepository.Verify(x => x.RevokedWebPortalAccessAsync(It.IsAny<string>(), It.IsAny<string>()));
            userRepository.Verify(x => x.Delete(It.IsAny<User>()));
        }
    }
}