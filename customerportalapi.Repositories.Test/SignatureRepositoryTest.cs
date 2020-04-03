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
                Content = new StringContent("{    \"version\": null,    \"statusCode\": 200,    \"message\": null,    \"isError\": false,    \"responseException\": null,    \"result\": [        {            \"_id\": \"5e7cefb115386f7920c1ded8\",            \"signatureResult\": {                \"id\": \"05dbb688-169a-4c67-8642-9f5d31f260f6\",                \"created_at\": \"26/03/2020 19:08:51\",                \"data\": {                    \"01_bank_txtContractNumber\": \"RI19AMA2730001LU7000\",                    \"01_bank_txtCompany\": \"Trasters Self-Storage III, S.L\",                    \"01_bank_txtCif\": \"B67065540\",                    \"01_bank_txtAccountName\": \"Trasters Self-Storage III, S.L\",                    \"01_bank_txtAddress\": \"c/ Bravo Murillo 194\",                    \"01_bank_txtPostalCode\": \"28020 - Madrid\",                    \"01_bank_txtCountry\": \"España\",                    \"01_bank_txtClientName\": \"Jordi Giménez\",                    \"01_bank_txtClientAddress\": \"c/ Balmes 258, 3 1\",                    \"01_bank_txtClientPostalCode\": \"08017 - Barcelona\",                    \"01_bank_txtClientCountry\": \"España\",                    \"01_bank_txtIban\": \"ES72 1234 5678 9012 3456 7890\"                },                \"documents\": [                    {                        \"id\": \"5f733cdc-91bc-4696-b218-dbba298d8c8b\",                        \"created_at\": \"26/03/2020 19:08:51\",                        \"email\": \"jordi.gimenez@quantion.com\",                        \"events\": [                            {                                \"type\": \"document_completed\",                                \"created_at\": \"2020-03-26T15:52:29+0000\"                            },                            {                                \"type\": \"document_completed\",                                \"created_at\": \"2020-03-26T15:52:29+0000\"                            },                            {                                \"type\": \"document_completed\",                                \"created_at\": \"2020-03-26T15:52:29+0000\"                            },                            {                                \"type\": \"document_completed\",                                \"created_at\": \"2020-03-26T15:52:29+0000\"                            }                        ],                        \"name\": \"Jordi\",                        \"status\": \"completed\",                        \"file\": {                            \"name\": \"SEPA_BankAccountOrder_Template.pdf\",                            \"pages\": 0,                            \"size\": 122803                        }                    }                ]            },            \"store\": \"Sant  Just\",            \"documents\": [                {                    \"documentType\": 1,                    \"documentNumber\": \"RT1234567890\",                    \"bankAccountOrderNumber\": \"12345678\",                    \"bankAccountName\": \"Bluespace\",                    \"smContractCode\": \"RI19AMA2730001LU7000\"                }            ],            \"units\": [],            \"userIdentification\": \"B37383940Q\",            \"recipients\": [                {                    \"recipientEmail\": \"jordi.gimenez@quantion.com\",                    \"recipientPhone\": null                }            ],            \"signatureChannel\": \"email\",            \"signatureType\": \"advanced\",            \"accountType\": 1,            \"accountDni\": \"37383940Q\",            \"signatureEvents_Url\": null,            \"signatureEndProcess_Url\": \"https://localhost:44332/api/events\"        }    ]}")
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
