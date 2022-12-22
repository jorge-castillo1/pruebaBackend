using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
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

            db.Setup(x => x.CheckFeature(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 24)).Returns(24).Verifiable();

            db.Setup(x => x.CheckFeature(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 3)).Returns(3).Verifiable();

            db.Setup(x => x.CheckFeature(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>()).Verifiable();



            return db;
        }
    }
}
