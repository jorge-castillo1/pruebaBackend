using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class WebTemplatesRepositoryTest
    {
        private Mock<IMongoCollectionWrapper<WebTemplate>> _webTemplates;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            builder.Build();

            _webTemplates = new Mock<IMongoCollectionWrapper<WebTemplate>>();
            _webTemplates.Setup(x => x.FindOne(It.IsAny<Expression<Func<WebTemplate, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<WebTemplate>
                {
                    new WebTemplate
                    {
                        Code = (int)WebTemplateTypes.LegalNotice,
                        Language = "Fake language",
                        Data = "Fake data"
                    }
                });
            _webTemplates.Setup(x => x.FindAll(It.IsAny<Expression<Func<WebTemplate, bool>>>())).Returns(
                new List<WebTemplate>
                {
                    new WebTemplate
                    {
                        Code = (int) WebTemplateTypes.LegalNotice,
                        Language = "Fake language",
                        Data = "Fake data"
                    },
                    new WebTemplate
                    {
                        Code = (int)WebTemplateTypes.PersonalDataProtection,
                        Language = "Fake language",
                        Data = "Fake data"
                    }
                });
        }

        [TestMethod]
        public void AlRecuperarTodasLasPlantillasExistentes_NoSeProducenErrores()
        {
            //Arrange

            //Act
            WebTemplateRepository webTemplateRepository = new WebTemplateRepository(_webTemplates.Object);
            var templates = webTemplateRepository.GetTemplates();

            //Assert
            Assert.IsNotNull(templates);
            Assert.AreEqual(2, templates.Count);
        }

        [TestMethod]
        public void AlRecuperarUnaPlantillaExistente_NoSeProducenErrores()
        {
            //Arrange

            //Act
            WebTemplateRepository webTemplateRepository = new WebTemplateRepository(_webTemplates.Object);
            var template = webTemplateRepository.GetTemplate((int)WebTemplateTypes.LegalNotice, "Fake Language");

            //Assert
            Assert.AreEqual((int)WebTemplateTypes.LegalNotice, template.Code);
            Assert.AreEqual("Fake language", template.Language);
        }

        [TestMethod]
        public void AlRecuperarUnaPlantillaConCodigoInexistente_NoSeProducenErrores()
        {
            //Arrange
            Mock<IMongoCollectionWrapper<WebTemplate>> webTemplateInvalid = new Mock<IMongoCollectionWrapper<WebTemplate>>();
            webTemplateInvalid.Setup(x => x.FindOne(It.IsAny<Expression<Func<WebTemplate, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<WebTemplate>());

            //Act
            WebTemplateRepository webTemplateRepository = new WebTemplateRepository(webTemplateInvalid.Object);
            var template = webTemplateRepository.GetTemplate(-1, "Fake language");

            //Assert
            Assert.IsNull(template.Data);
        }
    }
}
