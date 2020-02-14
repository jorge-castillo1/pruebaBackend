using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ContractRepositoryMock
    {
        public static Mock<IContractRepository> ContractRepository()
        {
            var db = new Mock<IContractRepository>();
            db.Setup(x => x.GetContractsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<Contract>
            {
                new Contract
                {
                   ContractNumber = "1234567890",
                   ContractDate = "01/01/2020",
                   Store = "Fake Store",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude"
                   }
                },
                new Contract
                {
                   ContractNumber = "1234567891",
                   ContractDate = "01/01/2019",
                   Store = "Fake Store",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude"
                   }
                },
                new Contract
                {
                   ContractNumber = "1234567892",
                   ContractDate = "01/01/2020",
                   Store = "Fake Store 2",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude"
                   }
                },
                new Contract
                {
                   ContractNumber = "1234567893",
                   ContractDate = "01/01/2020",
                   Store = "Fake Store 2",
                   StoreData = new Store
                   {
                       StoreName = "Fake Store",
                       Telephone = "Fake telephone",
                       CoordinatesLatitude = "Fake CoordinatesLatitude",
                       CoordinatesLongitude = "Fake CoordinatesLongitude"
                   }
                },
            })).Verifiable();

            return db;
        }
    }
}
