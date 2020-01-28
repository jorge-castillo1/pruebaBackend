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

            return db;
        }
    }
}
