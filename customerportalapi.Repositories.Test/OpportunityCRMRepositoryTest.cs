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
    public class OpportunityCRMepositoryTest
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
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClient"))
                .Returns(() =>
                {
                    return _handler.CreateClient();
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"OpportunityId\": \"fake id\"}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            OpportunityCRMRepository repository = new OpportunityCRMRepository(_configurations, _clientFactory);
            var result = repository.GetOpportunity("fake id").Result;

            //Assert
            Assert.IsNotNull(result);
        }

    }
}
