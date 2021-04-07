using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class StoreRepositoryMock
    {
        public static Mock<IStoreRepository> StoreRepository()
        {
            var db = new Mock<IStoreRepository>();
            db.Setup(x => x.GetStoresAsync()).Returns(Task.FromResult(new List<Store>
            {
                new Store()
            })).Verifiable();

            db.Setup(x => x.GetStoreAsync(It.IsAny<string>())).Returns(Task.FromResult(new Store()
                {
                    StoreCode = "FAKESTORECODE",
                    StoreName = "Fake name",
                    CompanyCif = "FAKE123456",
                    CompanyName = "Fake Company Name S.L.",
                    CompanySocialAddress = "Fake Street, 10",
                    Country = "FAKE",
                    City = "FAKE",
                    CountryCode = "ES",
                    OpeningDaysFirst = "01/02/2020",
                    OpeningDaysLast = "01/02/2020",
                    OpeningHoursFrom = "8",
                    OpeningHoursTo = "20",
                    EmailAddress1 = "fake1@email.com",
                    EmailAddress2 = "fake2@email.com"
            }
            )).Verifiable();

            return db;
        }

        public static Mock<IStoreRepository> StoreRepositoryWithoutEmail()
        {
            var db = new Mock<IStoreRepository>();
            db.Setup(x => x.GetStoresAsync()).Returns(Task.FromResult(new List<Store>
            {
                new Store()
            })).Verifiable();

            db.Setup(x => x.GetStoreAsync(It.IsAny<string>())).Returns(Task.FromResult(new Store()
            {
                StoreCode = "FAKESTORECODE",
                StoreName = "Fake name",
                CompanyCif = "FAKE123456",
                CompanyName = "Fake Company Name S.L.",
                CompanySocialAddress = "Fake Street, 10",
                Country = "FAKE",
                City = "FAKE",
                CountryCode = "ES",
                OpeningDaysFirst = "01/02/2020",
                OpeningDaysLast = "01/02/2020",
                OpeningHoursFrom = "8",
                OpeningHoursTo = "20"
            }
            )).Verifiable();

            return db;
        }
    }
}
