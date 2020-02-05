using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class IdentityRepositoryTest
    {
        IConfiguration _configurations;
        System.Net.Http.IHttpClientFactory _clientFactory;
        Mock<HttpMessageHandler> _handler;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _handler = new Mock<HttpMessageHandler>();
            _clientFactory = _handler.CreateClientFactory();
        }

        [TestMethod]
        public void AlHacerLlamadaExternaDeAuthorizacion_NoSeProducenErrores()
        {
            //Arrange
            Login credentials = new Login();
            credentials.Username = "Fake User";
            credentials.Password = "Fake Password";

            Mock.Get(_clientFactory).Setup(x => x.CreateClient("identityClient"))
                .Returns(() =>
                {
                    HttpClient client = _handler.CreateClient();
                    client.BaseAddress = new Uri("http://fakeUri");
                    return client;
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"id_token\": \"FakeId\", \"access_token\": \"Fake AccessToken\"}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            IdentityRepository repository = new IdentityRepository(_configurations, _clientFactory);
            Token result = repository.Authorize(credentials).Result;

            //Assert
            Assert.AreEqual("FakeId", result.IdToken);
            Assert.AreEqual("Fake AccessToken", result.AccesToken);
        }

        [TestMethod]
        public void AlHacerLlamadaExternaDeValidacion_NoSeProducenErrores()
        {
            //Arrange
            string token = "Fake access token";

            Mock.Get(_clientFactory).Setup(x => x.CreateClient("identityClient"))
                .Returns(() =>
                {
                    HttpClient client = _handler.CreateClient();
                    client.BaseAddress = new Uri("http://fakeUri");
                    return client;
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"id_token\": \"FakeId\", \"access_token\": \"Fake AccessToken\"}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            IdentityRepository repository = new IdentityRepository(_configurations, _clientFactory);
            Token result = repository.Validate(token).Result;

            //Assert
            Assert.AreEqual("FakeId", result.IdToken);
            Assert.AreEqual("Fake AccessToken", result.AccesToken);
        }

        [TestMethod]
        public void AlHacerLlamadaExternaDeCreacionUsuario_NoSeProducenErrores()
        {
            //Arrange
            UserIdentity newuser = new UserIdentity();
            newuser.UserName = "Fake userName";
            newuser.Password = "Fake Password";
            newuser.Emails = new List<EmailAccount>() {
                new EmailAccount(){
                    Primary = true,
                    Value = "Fake Email",
                    Type = "Fake type"
                }
            };
            
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("identityClient"))
                .Returns(() =>
                {
                    HttpClient client = _handler.CreateClient();
                    client.BaseAddress = new Uri("http://fakeUri");
                    return client;
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"id\": \"FakeId\", \"userName\": \"Fake userName\"}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            IdentityRepository repository = new IdentityRepository(_configurations, _clientFactory);
            var result = repository.AddUser(newuser).Result;

            //Assert
            result.UserName = "Fake userName";
        }

        [TestMethod]
        public void AlHacerLlamadaExternaDeActualizacionUsuario_NoSeProducenErrores()
        {
            //Arrange
            UserIdentity newuser = new UserIdentity();
            newuser.ID = "Fake ID";
            newuser.UserName = "Fake userName";
            newuser.Password = "Fake Password";
            newuser.Emails = new List<EmailAccount>() {
                new EmailAccount(){
                    Primary = true,
                    Value = "Fake Email",
                    Type = "Fake type"
                }
            };

            Mock.Get(_clientFactory).Setup(x => x.CreateClient("identityClient"))
                .Returns(() =>
                {
                    HttpClient client = _handler.CreateClient();
                    client.BaseAddress = new Uri("http://fakeUri");
                    return client;
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"id\": \"FakeId\", \"userName\": \"Fake userName\"}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            IdentityRepository repository = new IdentityRepository(_configurations, _clientFactory);
            var result = repository.UpdateUser(newuser).Result;

            //Assert
            result.UserName = "Fake userName";
        }
    }
}
