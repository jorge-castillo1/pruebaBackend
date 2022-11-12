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
    public class GoogleCaptchaRepositoryTest
    {
        private IConfigurationRoot _configurations;
        IHttpClientFactory _clientFactory;
        Mock<HttpMessageHandler> _handler;
        Mock<ILogger<GoogleCaptchaRepository>> _logger;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _handler = new Mock<HttpMessageHandler>();
            _clientFactory = _handler.CreateClientFactory();
            _logger = new Mock<ILogger<GoogleCaptchaRepository>>();
        }

        [TestMethod]
        public void AlHacerLlamadaExternaDeValidacion_NoSeProducenErrores()
        {
            //Arrange
            string token = "FakeId";

            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientCaptcha"))
                .Returns(() =>
                {
                    HttpClient client = _handler.CreateClient();
                    client.BaseAddress = new Uri("http://fakeUri");
                    return client;
                });

            var response = new HttpResponseMessage
            {
                Content = new StringContent("{\"success\": \"true\", \"challenge_ts\": \"2050-09-08T19:01:55.714942+03:00\", \"hostname\": \"Fake hostname\"}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            //Act
            GoogleCaptchaRepository repository = new GoogleCaptchaRepository(_configurations, _clientFactory, _logger.Object);
            bool result = repository.IsTokenValid(token).Result;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}