using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;

namespace customerportalapi.Services.Test.FakeData
{
    public static class GoogleCaptchaRepositoryMock
    {
        public static Mock<IGoogleCaptchaRepository> GoogleCaptchaRepository()
        {
            var db = new Mock<IGoogleCaptchaRepository>();
            db.Setup(x => x.IsTokenValid(It.IsAny<string>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }
    }
}
