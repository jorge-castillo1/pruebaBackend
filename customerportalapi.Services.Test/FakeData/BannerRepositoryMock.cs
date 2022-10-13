using customerportalapi.Repositories.Interfaces;
using Moq;

namespace customerportalapi.Services.Test.FakeData
{
    public static class BannerRepositoryMock
    {
        public static Mock<IBannerImageRepository> BanerRepository()
        {
            var db = new Mock<IBannerImageRepository>();

            db.Setup(x => x.GetUrlImage("ES", "ES")).Returns("").Verifiable();
            return db;
        }
    }
}
