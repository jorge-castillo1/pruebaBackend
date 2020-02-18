using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ProcessRepositoryMock
    {
        public static Mock<IProcessRepository> ProcessRepository()
        {
            var db = new Mock<IProcessRepository>();
            db.Setup(x => x.Create(It.IsAny<Process>())).Returns(Task.FromResult(true)).Verifiable();

            //db.Setup(x => x.Create(It.IsAny<Process>())).Returns(Task.FromResult(new Process()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Username = "fake user",
            //    ContractNumber = "fake contract",
            //    DocumentId = "fake document id",
            //    ProcessType = ProcessTypes.PaymentMethodChangeBank,
            //    ProcessStatus = ProcessStatuses.Pending,
            //    CreationDate = System.DateTime.Now
            //})).Verifiable();

            return db;
        }
    }
}
