using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class AccountSMRepositoryMock
    {
        public static Mock<IAccountSMRepository> AccountSMRepository()
        {
            var db = new Mock<IAccountSMRepository>();
            db.Setup(x => x.AddBankAccountAsync(It.IsAny<SMBankAccount>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }
    }
}
