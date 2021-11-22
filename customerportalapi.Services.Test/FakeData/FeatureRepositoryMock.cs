using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class FeatureRepositoryMock
    {
        public static Mock<IFeatureRepository> FeatureRepository()
        {
            var db = new Mock<IFeatureRepository>();
            db.Setup(x => x.CheckFeatureByNameAndEnvironment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true).Verifiable();

            db.Setup(x => x.Create(It.IsAny<Feature>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }
    }
}
