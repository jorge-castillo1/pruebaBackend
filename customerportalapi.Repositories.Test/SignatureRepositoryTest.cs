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
    public class SignatureRepositoryTest
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
            Mock.Get(_clientFactory).Setup(x => x.CreateClient("httpClientSignature"))
                .Returns(() =>
                {
                    var hnd = _handler.CreateClient();
                    hnd.BaseAddress = new System.Uri("http://fakeuri");
                    return hnd;
                });
            
            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ \"result\": { \"signatureprocess\": [{ \"store\": \"fake store\"," +
                "\"useridentification\": \"fake user\"}]}}")
            };
            _handler.SetupAnyRequest()
                .ReturnsAsync(response);

            SignatureSearchFilter filter = new SignatureSearchFilter();
            filter.Filters.SignatureId = "fake signature id";

            //Act
            SignatureRepository repository = new SignatureRepository(_configurations, _clientFactory);
            List<SignatureProcess> result = await repository.SearchSignaturesAsync(filter);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
    }
}
