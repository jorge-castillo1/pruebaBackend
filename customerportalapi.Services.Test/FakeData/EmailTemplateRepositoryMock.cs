using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Services.Test.FakeData
{
    public static class EmailTemplateRepositoryMock
    {
        public static Mock<IEmailTemplateRepository> EmailTemplateRepository()
        {
            var db = new Mock<IEmailTemplateRepository>();
            db.Setup(x => x.getTemplate(It.IsAny<int>(), It.IsAny<string>())).Returns(new EmailTemplate()
            {
                code = 0,
                subject = "fake subject",
                body = "fake body",
                language = "fake lang",
            }).Verifiable();

            db.Setup(x => x.getTemplate(It.IsAny<int>(), "en")).Returns(new EmailTemplate()
            {
                _id = "fake id",
                code = 0,
                subject = "fake subject",
                body = "fake body",
                language = "fake lang",
            }).Verifiable();

            return db;
        }
    }
}
