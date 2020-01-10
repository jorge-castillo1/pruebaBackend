using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class UserRepositoryTest
    {
        IConfigurationRoot _configurations;
        Mock<IMongoCollectionWrapper<User>> _users;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _users = new Mock<IMongoCollectionWrapper<User>>();
            _users.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<User>() {
                new User() {
                    dni = "12345678Z",
                    email = "fakeuser@email.com"
                }});
            _users.Setup(x => x.InsertOne(It.IsAny<User>())).Verifiable();
            _users.Setup(x => x.ReplaceOne(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>())).Returns(new Mock<ReplaceOneResult>().Object).Verifiable();
            _users.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<User>>())).Returns(Task.FromResult(new Mock<DeleteResult>().Object)).Verifiable();
        }

        [TestMethod]
        public void AlRecuperarUnUsuarioExistente_NoSeProducenErrores()
        {
            //Arrange
            User user = new User();
            UserRepository _userRepository = new UserRepository(_configurations, _users.Object);
            
            //Act
            user = _userRepository.getCurrentUser("12345678Z");

            //Assert
            Assert.AreEqual("fakeuser@email.com", user.email);
        }

        [TestMethod]
        public void AlRecuperarUnUsuarioInexistente_NoSeProducenErrores()
        {
            //Arrange
            User user = new User();
            Mock<IMongoCollectionWrapper<User>> _usersInvalid = new Mock<IMongoCollectionWrapper<User>>();
            _usersInvalid.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<User>());

            UserRepository _userRepository = new UserRepository(_configurations, _usersInvalid.Object);

            //Act
            user = _userRepository.getCurrentUser("00000000Z");

            //Assert
            Assert.IsNull(user.dni);
        }

        [TestMethod]
        public void AlActualizarUnUsuario_NoSeProducenErrores()
        {
            //Arrange
            UserRepository _userRepository = new UserRepository(_configurations, _users.Object);
            User testUser =_userRepository.getCurrentUser("12345678Z");
            
            //Act
            testUser.email = "fakeuser@changed.com";
            User modifiedUser = _userRepository.update(testUser);

            //Assert
            Assert.AreEqual("fakeuser@changed.com", modifiedUser.email);
        }

        [TestMethod]
        public void AlEliminarUnUsuario_NoSeProducenErrores()
        {
            User testUser = GetTestUser();

            UserRepository _userRepository = new UserRepository(_configurations, _users.Object);
            _userRepository.delete(testUser);
        }

        private static User GetTestUser()
        {
            User testUser = new User();
            testUser.dni = "12345678Z";
            testUser.email = "fakeuser@email.com";
            testUser.language = "en";
            testUser.profilepicture = "iVBORw0KGgoAAAANSUhEUgAAAfQAAAH0CAMAAAD8CC+4AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjM4REY1ODNDOTk1MTExRTBCQzMyQkM5MEU2MTdEMUM1IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjM4REY1ODNEOTk1MTExRTBCQzMyQkM5MEU2MTdEMUM1Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MzhERjU4M0E5OTUxMTFFMEJDMzJCQzkwRTYxN0QxQzUiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MzhERjU4M0I5OTUxMTFFMEJDMzJCQzkwRTYxN0QxQzUiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz5z5x1CAAACvlBMVEVJf7xKf7xKgLxK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";
            return testUser;
        }
    }
}
