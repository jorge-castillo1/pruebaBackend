using System;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using System.Net.Http;

namespace customerportalapi.Repositories.Test
{

    [TestClass]
    public class SitesRepositoryTest
    {
        private IConfiguration _configurations;
        private IHttpClientFactory _clientFactory;
        private Mock<HttpMessageHandler> _handler;

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
        public void AlHacerUnaLlamadaGetExternaDeDatos_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientSM"))
                .Returns(() => _handler.CreateClient());

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": [{ \"siteId\": \"fake siteId\"}]}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            SitesRepository repository = new SitesRepository(_configurations, _clientFactory);
            var result = repository.GetSmSitesAsync().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "No se ha producido la excepción esperada.")]
        public void AlHacerUnaLlamadaGetExternaDeDatos_SeProduceUnaExcepcion()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientSM"))
                .Returns(() => _handler.CreateClient());

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            SitesRepository repository = new SitesRepository(_configurations, _clientFactory);
            var result = repository.GetSmSitesAsync().Result;
        }
    }
}
