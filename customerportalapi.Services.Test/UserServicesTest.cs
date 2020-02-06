using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private Mock<IConfiguration> _config;


        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailtemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _config = new Mock<IConfiguration>();
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.GetProfileAsync(dni);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarUnUsuarioExistente_DevuelvePerfil()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            Profile usuario = await service.GetProfileAsync(dni);

            //Assert
            Assert.IsNotNull(usuario);
            Assert.AreEqual("fake profile image", usuario.Avatar);
            Assert.AreEqual("fake name", usuario.Fullname);
            Assert.IsTrue(usuario.EmailAddress1Principal);
            Assert.IsFalse(usuario.EmailAddress2Principal);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlActualizarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlActualizarUnUsuarioExistente_ConEmailPrincipal2Invalido_SeProducenErrores()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            profile.Language = "new language";
            profile.EmailAddress1 = "fake email 1";
            profile.EmailAddress1Principal = false;
            profile.EmailAddress2 = string.Empty;
            profile.EmailAddress2Principal = true;
            profile.Avatar = "new profile image";

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
            _userRepository.Verify(x => x.GetCurrentUser(It.IsAny<string>()));
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
            _profileRepository.Verify(x => x.UpdateProfileAsync(It.IsAny<Profile>()));

            Assert.AreEqual("fake Address modified", result.Address);
            Assert.AreEqual("fake lang modified", result.Language);
            Assert.AreEqual("fake email 1 modified", result.EmailAddress1);
            Assert.IsTrue(result.EmailAddress1Principal);
            Assert.IsFalse(result.EmailAddress2Principal);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.InviteUserAsync(invitation);

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Update(It.IsAny<User>()));
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.Create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlConfirmarUnUsuarioSinToken_DevuelveExcepcion()
        {
            //Arrange
            string invitationToken = string.Empty;

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.ConfirmUserAsync(invitationToken);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_Activo_DevuelveFalse()
        {
            //Arrange
            string invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.Invalid_ActiveUserByToken_Repository();
            UserServices service = new UserServices(userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AlConfirmarUnUsuarioExistente_NoActivo_ActualizaUsuario()
        {
            //Arrange
            string invitationToken = "8e8b9c6c-8943-4482-891d-b92d7414d283";

            //Act
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUserByToken_Repository();
            UserServices service = new UserServices(userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.ConfirmUserAsync(invitationToken);

            //Assert
            Assert.IsTrue(result);
            _profileRepository.Verify(x => x.ConfirmedWebPortalAccessAsync(It.IsAny<string>()));
            userRepositoryInvalid.Verify(x => x.Update(It.IsAny<User>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlDesinvitarUnUsuarioSinDni_DevuelveExcepcion()
        {
            //Arrange
            string dni = string.Empty;

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.UnInviteUserAsync(dni);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada.")]
        public async Task AlDesinvitarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            await service.UnInviteUserAsync(dni);
        }

        [TestMethod]
        public async Task AlDesinvitarUnUsuarioExistente_NoActivo_DevuelveFalse()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            Mock<IUserRepository> userRepositoryInvalid = UserRepositoryMock.Valid_InActiveUser_Repository();
            UserServices service = new UserServices(userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.UnInviteUserAsync(dni);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AlDesinvitarUnUsuarioExistente_Activo_ActualizaUsuario()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            Mock<IUserRepository> userRepository = UserRepositoryMock.ValidUserRepository();
            UserServices service = new UserServices(userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object, _identityRepository.Object, _config.Object);
            bool result = await service.UnInviteUserAsync(dni);

            //Assert
            Assert.IsTrue(result);
            _profileRepository.Verify(x => x.RevokedWebPortalAccessAsync(It.IsAny<string>()));
            userRepository.Verify(x => x.Update(It.IsAny<User>()));
        }
    }
}
