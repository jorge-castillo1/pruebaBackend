using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class OpportunityCRMRepositoryMock
    {
        public static Mock<IOpportunityCRMRepository> OpportunityCRMRepository()
        {
            var db = new Mock<IOpportunityCRMRepository>();
            db.Setup(x => x.GetOpportunity(It.IsAny<string>())).Returns(Task.FromResult(new OpportunityCRM
            {     
                OpportunityId = "1234567890",
                ExpectedMoveIn = "01/01/2020",
                
            })).Verifiable();

            return db;
        }

    }

}
