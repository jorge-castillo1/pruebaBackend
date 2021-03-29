using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class UnitLocationRepositoryMock
    {
        public static Mock<IUnitLocationRepository> UnitLocationRepository()
        {
            var db = new Mock<IUnitLocationRepository>();
            db.Setup(x => x.GetBySizeCode(It.IsAny<string>())).Returns(
                new UnitLocation()
                {
                    Id = "605dd201d7edd1a958574d43",
                    SiteCode = "Fake-SiteCode",
                    SizeCode = "Fake-SizeCode",
                    Description = "Fake-Description"
                }
            ).Verifiable();

            db.Setup(x => x.Update(It.IsAny<UnitLocation>())).Returns(
                new UnitLocation()
                {
                    Id = "605dd201d7edd1a958574d43",
                    SiteCode = "Fake-SiteCode",
                    SizeCode = "Fake-SizeCode",
                    Description = "Fake-Description-Update"
                }
            ).Verifiable();

            db.Setup(x => x.Create(It.IsAny<UnitLocation>())).Returns(
                Task.FromResult(true)
            ).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<UnitLocation>())).Returns(
                Task.FromResult(true)
            ).Verifiable();

            db.Setup(x => x.Find(It.IsAny<UnitLocationSearchFilter>())).Returns(
                new List<UnitLocation>()
                {
                    new UnitLocation()
                    {
                        Id = "605dd201d7edd1a958574d43",
                        SiteCode = "Fake-SiteCode",
                        SizeCode = "Fake-SizeCode",
                        Description = "Fake-Description"
                    }
                }
            ).Verifiable();

            return db;
        }
    }
}
