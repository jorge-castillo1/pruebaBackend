using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class EmailTemplatesRepositoryTest
    {
        IConfigurationRoot _configurations;
        Mock<IMongoCollectionWrapper<EmailTemplate>> _emailtemplates;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _emailtemplates = new Mock<IMongoCollectionWrapper<EmailTemplate>>();
            _emailtemplates.Setup(x => x.FindOne(It.IsAny<Expression<Func<EmailTemplate, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<EmailTemplate>() {
                new EmailTemplate() {
                    code = (int)EmailTemplateTypes.WelcomeEmailStandard,
                    subject = "Fake subject",
                    body = "Fake html body",
                    language = "Fake language"
                }});
        }

        [TestMethod]
        public void AlRecuperarUnaPlantillaExistente_NoSeProducenErrores()
        {
            //Arrange
            EmailTemplate template = new EmailTemplate();

            //Act
            EmailTemplateRepository _emailTemplateRepository = new EmailTemplateRepository(_configurations, _emailtemplates.Object);
            template = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.WelcomeEmailStandard, "Fake Language");

            //Assert
            Assert.AreEqual((int)EmailTemplateTypes.WelcomeEmailStandard, template.code);
            Assert.AreEqual("Fake language", template.language);
        }

        [TestMethod]
        public void AlRecuperarUnaPlantillaConCodigoInexistente_NoSeProducenErrores()
        {
            //Arrange
            EmailTemplate template = new EmailTemplate();
            Mock<IMongoCollectionWrapper<EmailTemplate>> _templateInvalid = new Mock<IMongoCollectionWrapper<EmailTemplate>>();
            _templateInvalid.Setup(x => x.FindOne(It.IsAny<Expression<Func<EmailTemplate, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<EmailTemplate>());

            //Act
            EmailTemplateRepository _emailTemplateRepository = new EmailTemplateRepository(_configurations, _templateInvalid.Object);
            template = _emailTemplateRepository.getTemplate(-1, "Fake Language");

            //Assert
            Assert.IsNull(template.subject);
        }
    }
}
