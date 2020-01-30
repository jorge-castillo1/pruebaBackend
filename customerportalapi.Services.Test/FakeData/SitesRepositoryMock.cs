using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class SitesRepositoryMock
    {
        public static Mock<ISitesRepository> SitesRepository()
        {
            var db = new Mock<ISitesRepository>();
            db.Setup(x => x.GetSmSitesAsync()).Returns(Task.FromResult(new List<SmSite>
            {
                new SmSite
                {
                    SiteId = "Fake SiteId"
                }
            })).Verifiable();

            return db;
        }
    }
}
