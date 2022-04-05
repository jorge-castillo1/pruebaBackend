using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    //    [TestClass]
    public static class NewUserRepositoryMock
    {
        public static Mock<INewUserRepository> ValidNewUserRepository()
        {
            var db = new Mock<INewUserRepository>();
            db.Setup(x => x.SaveNewUser(It.IsAny<NewUser>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }


    }
}
