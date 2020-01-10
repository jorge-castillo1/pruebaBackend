using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class MailRepositoryMock
    {
        public static Mock<IMailRepository> MailRepository()
        {
            var db = new Mock<IMailRepository>();
            db.Setup(x => x.Send(It.IsAny<Email>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }
    }
}
