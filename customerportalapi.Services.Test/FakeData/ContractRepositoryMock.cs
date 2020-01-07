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
            db.Setup(x => x.GetContractsAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Contract>()
            {
                new Contract() {
                   ContractNumber = "1234567890",
                   ContractDate = "01/01/2020",
                   ContractStatus = 1,
                   Store = "Fake Store"
                },
                new Contract() {
                   ContractNumber = "1234567891",
                   ContractDate = "01/01/2019",
                   ContractStatus = 2,
                   Store = "Fake Store"
                },
                new Contract() {
                   ContractNumber = "1234567892",
                   ContractDate = "01/01/2020",
                   ContractStatus = 1,
                   Store = "Fake Store 2"
                },
                new Contract() {
                   ContractNumber = "1234567893",
                   ContractDate = "01/01/2020",
                   ContractStatus = 1,
                   Store = "Fake Store 2"
                },
            })).Verifiable();

            return db;
        }
    }
}
