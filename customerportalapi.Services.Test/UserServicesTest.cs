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

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _profileRepository = ProfileRepositoryMock.ProfileRepository();
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "No se ha producido la excepción esperada.")]
        public async Task AlSolicitarUnUsuarioNoExistente_SeProduceUnaExcepcion()
        {
            //Arrange
            string dni = "12345678A";
            Mock<IUserRepository> _userRepositoryInvalid = UserRepositoryMock.InvalidUserRepository();

            //Act
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object);
            await service.GetProfileAsync(dni);

            //Assert
        }

        [TestMethod]
        public async Task AlSolicitarUnUsuarioExistente_DevuelvePerfil()
        {
            //Arrange
            string dni = "12345678A";

            //Act
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object);
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
            UserServices service = new UserServices(_userRepositoryInvalid.Object, _profileRepository.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object);
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
            UserServices service = new UserServices(_userRepository.Object, _profileRepository.Object);
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
    }
}
