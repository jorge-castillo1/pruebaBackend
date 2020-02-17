using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using Moq.Contrib.HttpClient;
using System.Collections.Generic;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class ContractRepositoryTest
    {
        IConfiguration _configurations;
        IHttpClientFactory _clientFactory;
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
        public void AlHacerUnaLlamadaGetExternaDeDatosQueDevuelveEntidad_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCRM"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": [{ \"contractnumber\": \"fake contract number\"}]}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ContractRepository repository = new ContractRepository(_configurations, _clientFactory);
            List<Contract> result = repository.GetContractsAsync("fake dni", "fake customertype").Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public void AlHacerUnaLlamadaGetExternaDeDatosQueDevuelveLista_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCRM"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": [{ \"contractnumber\": \"fake contract number\"}, { \"contractnumber\": \"fake contract number 2\"}]}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ContractRepository repository = new ContractRepository(_configurations, _clientFactory);
            List<Contract> result = repository.GetContractsAsync("fake dni", "fake customertype").Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
        }

    }
}
