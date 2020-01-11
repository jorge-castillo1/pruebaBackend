using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class UserServicesTest
    {
        Mock<IUserRepository> _userRepository;
        Mock<IProfileRepository> _profileRepository;
        Mock<IMailRepository> _mailRepository;
        Mock<IEmailTemplateRepository> _emailtemplateRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
            _mailRepository = MailRepositoryMock.MailRepository();
            _emailtemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            await service.GetProfileAsync(dni);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarUnUsuarioExistente_DevuelvePerfil()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            Profile usuario = await service.GetProfileAsync(dni);

            //Assert
            Assert.IsNotNull(usuario);
            Assert.AreEqual("fake profile image", usuario.Avatar);
            Assert.AreEqual("fake name", usuario.Fullname);
            Assert.IsTrue(usuario.EmailAddress1Principal);
            Assert.IsFalse(usuario.EmailAddress2Principal);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
        public async Task AlActualizarUnUsuarioInexistente_SeProduceUnaExcepcion()
        {
            //Arrange
            Profile profile = new Profile();
            profile.DocumentNumber = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            Profile result = await service.UpdateProfileAsync(profile);

            //Assert
            _userRepository.Verify(x => x.getCurrentUser(It.IsAny<string>()));
            _userRepository.Verify(x => x.update(It.IsAny<User>()));
            _profileRepository.Verify(x => x.UpdateProfileAsync(It.IsAny<Profile>()));

            Assert.AreEqual("fake Address modified", result.Address);
            Assert.AreEqual("fake lang modified", result.Language);
            Assert.AreEqual("fake email 1 modified", result.EmailAddress1);
            Assert.IsTrue(result.EmailAddress1Principal);
            Assert.IsFalse(result.EmailAddress2Principal);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            await service.InviteUserAsync(invitation);

        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException), "No se ha producido la excepción esperada.")]
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.update(It.IsAny<User>()));
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object, _mailRepository.Object, _emailtemplateRepository.Object);
            bool result = await service.InviteUserAsync(invitation);

            //Assert
            Assert.IsTrue(result);
            _userRepositoryInvalid.Verify(x => x.create(It.IsAny<User>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

    }
}
