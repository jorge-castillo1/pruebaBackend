using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ContractSMRepositoryMock
    {
        public static Mock<IContractSMRepository> ContractSMRepository()
        {
            var db = new Mock<IContractSMRepository>();
            db.Setup(x => x.GetAccessCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new SMContract()
            {
                Password = "fake password",
                Contractnumber = "fake contract number"
            })).Verifiable();

            return db;
        }
    }
}
