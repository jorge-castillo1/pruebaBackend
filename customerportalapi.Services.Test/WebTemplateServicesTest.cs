using System.Collections.Generic;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class WebTemplateServicesTest
    {
        private Mock<IWebTemplateRepository> _webTemplateRepository;
        public Mock<IConfiguration> Config { get; private set; }

        [TestInitialize]
        public void Setup()
        {
            _webTemplateRepository = WebTemplateRepositoryMock.WebTemplateRepository();
            Config = new Mock<IConfiguration>();
        }

        [TestMethod]
        public async Task AlSolicitarTodasLasPlantillasExistentes_DevuelveLista()
        {
            //Arrange

            //Act
            WebTemplateServices service = new WebTemplateServices(_webTemplateRepository.Object);
            List<WebTemplate> templates = await service.GetTemplates();

            //Assert
            Assert.IsNotNull(templates);
        }

        [TestMethod]
        public async Task AlSolicitarUnaPlantillasExistente_DevuelvePlantilla()
        {
            //Arrange
            string language = "en";

            //Act
            WebTemplateServices service = new WebTemplateServices(_webTemplateRepository.Object);
            WebTemplate template = await service.GetTemplate(WebTemplateTypes.PersonalDataProtection, language);

            //Assert
            Assert.IsNotNull(template);
        }
    }
}
