using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class LoginServicesTest
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IIdentityRepository> _identityRepository;
        private Mock<IEmailTemplateRepository> _emailtemplateRepository;
        private Mock<IMailRepository> _mailRepository;
        private IConfiguration _config;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
            _emailtemplateRepository = EmailTemplateRepositoryMock.EmailTemplateRepository();
            _mailRepository = MailRepositoryMock.MailRepository();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _config = builder.Build();
        }

        [TestMethod]
        public async Task AlSolicitarUnTokenConUnUsuarioValido_DevuelveJWTToken()
        {
            //Arrange
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token jwttoken = await service.GetToken(credentials);

            //Assert
            Assert.IsNotNull(jwttoken);
            Assert.AreEqual("Fake AccessToken", jwttoken.AccesToken);
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlSolicitarUnTokenConUnUsuarioInValido_DevuelveExcepcion()
        {
            //Arrange
            _userRepository = UserRepositoryMock.InvalidUserRepository();
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token jwttoken = await service.GetToken(credentials);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task LoginNoDisponibleParaUsuario_Con5IntentosFallidos_Y_Menos15Minutos()
        {
            //Arrange
            _userRepository = UserRepositoryMock.InvalidUserRepository_With5Attempts();
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token jwttoken = await service.GetToken(credentials);
        }

        [TestMethod]
        public async Task LoginDisponibleParaUsuario_Con5IntentosFallidos_Y_Mas15Minutos()
        {
            //Arrange
            _userRepository = UserRepositoryMock.ValidUserRepository_With5Attempts();
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token jwttoken = await service.GetToken(credentials);

            //Assert
            Assert.IsNotNull(jwttoken);
            Assert.AreEqual("Fake AccessToken", jwttoken.AccesToken);
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
        }

        [TestMethod]
        public async Task LoginDisponibleParaUsuario_ConMenos5Intentos()
        {
            //Arrange
            _userRepository = UserRepositoryMock.ValidUserRepository();
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token jwttoken = await service.GetToken(credentials);

            //Assert
            Assert.IsNotNull(jwttoken);
            Assert.AreEqual("Fake AccessToken", jwttoken.AccesToken);
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
        }

        [TestMethod]
        public async Task AlCambiarContraseña_SeValidaQueExistaUsuario()
        {
            //Arrange
            ResetPassword credentials = new ResetPassword();
            credentials.Username = "Fake User";
            credentials.OldPassword = "Fake Old";
            credentials.NewPassword = "Fake New";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            Token newToken = await service.ChangePassword(credentials);

            //Assert
            Assert.IsNotNull(newToken);
            Assert.AreEqual("Fake AccessToken", newToken.AccesToken);
            _userRepository.Verify(x => x.GetCurrentUser(It.IsAny<string>()));
            _identityRepository.Verify(x => x.UpdateUser(It.IsAny<UserIdentity>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlOlvidarContraseñaConUsernameInvalido_SeProduceUnError()
        {
            //Arrange
            string userName = "fakeUserName";

            //Act
            _userRepository = UserRepositoryMock.InvalidUserRepository();
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            await service.SendNewCredentialsAsync(userName);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException), "No se ha producido la excepción esperada")]
        public async Task AlOlvidarContraseñaSinAceptarInvitacion_SeProduceUnError()
        {
            //Arrange
            string userName = "fakeUserName";

            //Act
            _userRepository = UserRepositoryMock.Valid_InActiveUser_Repository();
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            await service.SendNewCredentialsAsync(userName);
        }

        [TestMethod]
        [ExpectedException(typeof(MockException), "No se ha producido la excepción esperada")]
        public async Task AlOlvidarContraseñaYNoexistePlantilla_NoSeEnviaUnCorreo()
        {
            //Arrange
            string userName = "fakeUserName";

            //Act
            _emailtemplateRepository = EmailTemplateRepositoryMock.Invalid_EmailTemplateRepository();
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            await service.SendNewCredentialsAsync(userName);

            //Assert verificar si se ha invocado a enviarcorreo
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }

        [TestMethod]
        public async Task AlOlvidarContraseña_SeEnviaUnCorreoMediantePlantilla()
        {
            //Arrange
            string userName = "fakeUserName";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object, _emailtemplateRepository.Object, _mailRepository.Object, _config);
            await service.SendNewCredentialsAsync(userName);

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()));
            _emailtemplateRepository.Verify(x => x.getTemplate(It.IsAny<int>(), It.IsAny<string>()));
            _mailRepository.Verify(x => x.Send(It.IsAny<Email>()));
        }
    }
}
