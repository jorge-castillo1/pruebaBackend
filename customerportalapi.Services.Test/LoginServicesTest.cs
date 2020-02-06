using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        
        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _identityRepository = IdentityRepositoryMock.IdentityRepository();
        }

        [TestMethod]
        public async Task AlSolicitarUnToken_DevuelveJWTToken()
        {
            //Arrange
            Login credentials = new Login();
            credentials.Username = "12345678A";
            credentials.Password = "12345678A";

            //Act
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object);
            Token jwttoken = await service.GetToken(credentials);

            //Assert
            Assert.IsNotNull(jwttoken);
            Assert.AreEqual("Fake AccessToken", jwttoken.AccesToken);
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
            LoginService service = new LoginService(_identityRepository.Object, _userRepository.Object);
            Token newToken = await service.ChangePassword(credentials);

            //Assert
            Assert.IsNotNull(newToken);
            Assert.AreEqual("Fake AccessToken", newToken.AccesToken);
            _userRepository.Verify(x => x.GetCurrentUser(It.IsAny<string>()));
            _identityRepository.Verify(x => x.UpdateUser(It.IsAny<UserIdentity>()));
        }
    }
}
