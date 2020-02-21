using customerportalapi.Entities;
using customerportalapi.Entities.enums;
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
            
            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()
            {
                new Process(){
                    Id = Guid.NewGuid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    DocumentId = "fake document id",
                    ProcessType = (int)ProcessTypes.PaymentMethodChangeBank,
                    ProcessStatus = (int)ProcessStatuses.Accepted,
                    CreationDate = System.DateTime.Now
                },
                new Process(){
                    Id = Guid.NewGuid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    DocumentId = "fake document id",
                    ProcessType = (int)ProcessTypes.PaymentMethodChangeBank,
                    ProcessStatus = (int)ProcessStatuses.Canceled,
                    CreationDate = System.DateTime.Now,
                    ModifiedDate = System.DateTime.Now
                }
            }).Verifiable();

            return db;
        }

        public static Mock<IProcessRepository> PendingSameProcessRepository()
        {
            var db = new Mock<IProcessRepository>();
            
            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()
            {
                new Process(){
                    Id = Guid.NewGuid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    DocumentId = "fake document id",
                    ProcessType = (int)ProcessTypes.PaymentMethodChangeBank,
                    ProcessStatus = (int)ProcessStatuses.Pending,
                    CreationDate = System.DateTime.Now
                }
            }).Verifiable();

            return db;
        }

        public static Mock<IProcessRepository> NoPendingSameProcessRepository()
        {
            var db = new Mock<IProcessRepository>();

            db.Setup(x => x.Create(It.IsAny<Process>())).Returns(Task.FromResult(true)).Verifiable();
            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()).Verifiable();

            return db;
        }

        public static Mock<IProcessRepository> NoResultsProcessRepository()
        {
            var db = new Mock<IProcessRepository>();

            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()).Verifiable();

            return db;
        }

        public static Mock<IProcessRepository> MoreThanOneResultProcessRepository()
        {
            var db = new Mock<IProcessRepository>();

            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()
            {
                new Process()
                {
                    Id = new Guid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    ProcessType = 1,
                    ProcessStatus = 0,
                    ModifiedDate = DateTime.Now
                },
                new Process()
                {
                    Id = new Guid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    ProcessType = 1,
                    ProcessStatus = 1,
                    ModifiedDate = DateTime.Now.AddDays(5)
                },
                new Process()
                {
                    Id = new Guid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    ProcessType = 1,
                    ProcessStatus = 2,
                    ModifiedDate = DateTime.Now.AddDays(2)
                }
            }).Verifiable();

            return db;
        }

        public static Mock<IProcessRepository> OneResultProcessRepository()
        {
            var db = new Mock<IProcessRepository>();

            db.Setup(x => x.Find(It.IsAny<ProcessSearchFilter>())).Returns(new List<Process>()
            {
                new Process()
                {
                    Id = new Guid().ToString(),
                    Username = "fake user",
                    ContractNumber = "fake contract",
                    ProcessStatus = 0
                }
            }).Verifiable();
            db.Setup(x => x.Update(It.IsAny<Process>())).Returns(new Process()
            {
                Id = new Guid().ToString(),
                Username = "fake user",
                ContractNumber = "fake contract",
                ProcessStatus = 1
            }).Verifiable();

            return db;
        }
    }
}
