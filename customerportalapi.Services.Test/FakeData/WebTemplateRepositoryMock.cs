using System.Collections.Generic;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using Moq;

namespace customerportalapi.Services.Test.FakeData
{
    public static class WebTemplateRepositoryMock
    {
        public static Mock<IWebTemplateRepository> WebTemplateRepository()
        {
            var db = new Mock<IWebTemplateRepository>();
            db.Setup(x => x.GetTemplate(It.IsAny<int>(), It.IsAny<string>())).Returns(new WebTemplate
            {
                Code = (int)WebTemplateTypes.LegalNotice,
                Language = "Fake language",
                Data = "Fake data"
            }).Verifiable();
            db.Setup(x => x.GetTemplate(It.IsAny<int>(), "en")).Returns(new WebTemplate
            {
                Id = "Fake id",
                Code = (int)WebTemplateTypes.PersonalDataProtection,
                Language = "en",
                Data = "Fake data"
            }).Verifiable();
            db.Setup(x => x.GetTemplates()).Returns(
                new List<WebTemplate> {
                    new WebTemplate
                    {
                        Id = "Fake id",
                        Code = (int)WebTemplateTypes.LegalNotice,
                        Language = "es",
                        Data = "Fake data"
                    },
                    new WebTemplate
                    {
                        Id = "Fake id",
                        Code = (int)WebTemplateTypes.PersonalDataProtection,
                        Language = "en",
                        Data = "Fake data"
                    }
                }).Verifiable();

            return db;
        }
    }
}
