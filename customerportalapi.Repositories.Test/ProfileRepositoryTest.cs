using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using System;
using System.Net.Http;

namespace customerportalapi.Repositories.Test
{

    [TestClass]
    public class ProfileRepositoryTest
    {
        IConfiguration _configurations;
        IHttpClientFactory _clientFactory;
        Mock<HttpMessageHandler> _handler;
        Mock<ILogger<ProfileRepository>> _logger;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _handler = new Mock<HttpMessageHandler>();
            _clientFactory = _handler.CreateClientFactory();
            _logger = new Mock<ILogger<ProfileRepository>>();
        }

        [TestMethod]
        public void AlHacerUnaLlamadaGetExternaDeDatos_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCRM"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"fullname\": \"fake profile\"}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ProfileRepository repository = new ProfileRepository(_configurations, _clientFactory, _logger.Object);
            var result = repository.GetProfileAsync("fake dni", "fake customertype").Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AlHacerUnaLlamadaSendExternaDeDatos_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCRM"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"fullname\": \"fake profile\"}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ProfileRepository repository = new ProfileRepository(_configurations, _clientFactory, _logger.Object);
            var result = repository.UpdateProfileAsync(new Entities.Profile()).Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AlHacerUnaLlamadaExternaDeDatosDePermisosDeContacto_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCRM"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"documentnumber\": \"123456789\", \"admincontact\": \"true\", \"supercontact\": \"false\"}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ProfileRepository repository = new ProfileRepository(_configurations, _clientFactory, _logger.Object);
            var result = repository.GetProfilePermissionsAsync("fake dni", "fake customertype").Result;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}
