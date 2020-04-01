using customerportalapi.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using Moq.Contrib.HttpClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class ContractSMRepositoryTest
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
        public async Task AlHacerUnaLlamadaGetExternaDeDatosQueDevuelveEntidad_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClient"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"contractnumber\": \"fake contract number\"," +
                "\"password\": \"fake password\"}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ContractSMRepository repository = new ContractSMRepository(_configurations, _clientFactory);
            SMContract result = await repository.GetAccessCodeAsync("fake contract number");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Password == "fake password");
        }

        [TestMethod]
        public async Task AlHacerUnaLlamadaGetExternaDeDatosQueDevuelveLista_NoDevuelveErrores()
        {
            //Arrange
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClient"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": [{ \"DocumentDate\": \"2020-01-16\"," +
                "\"Amount\": \"197.34\"}, { \"DocumentDater\": \"2020-01-16\"," +
                "\"Amount\": \"34.75\" }]}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            ContractSMRepository repository = new ContractSMRepository(_configurations, _clientFactory);
            List<Invoice> result = await repository.GetInvoicesAsync("fake contract number");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
        }
    }
}
