using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class IdentityRepositoryMock
    {
        public static Mock<IIdentityRepository> IdentityRepository()
        {
            var db = new Mock<IIdentityRepository>();
            db.Setup(x => x.Authorize(It.IsAny<Login>())).Returns(Task.FromResult(new Token()
            {
                IdToken = "FakeId",
                AccesToken = "Fake AccessToken"
            })).Verifiable();

            db.Setup(x => x.GetUser(It.IsAny<string>())).Returns(Task.FromResult(new UserIdentity()
            {
                ID = "Fake ID",
                UserName = "Fake UserName",
                Password = "Fake Password"
            })).Verifiable();

            db.Setup(x => x.UpdateUser(It.IsAny<UserIdentity>())).Returns(Task.FromResult(new UserIdentity()
            {
                ID = "Fake ID",
                UserName = "Fake UserName",
                Password = "Fake Password"
            })).Verifiable();

            db.Setup(x => x.AddUser(It.IsAny<UserIdentity>())).Returns(Task.FromResult(new UserIdentity()
            {
                ID = "Fake ID",
                UserName = "Fake UserName",
                Password = "Fake Password"
            })).Verifiable();

            db.Setup(x => x.FindGroup(It.IsAny<string>())).Returns(Task.FromResult(new GroupResults()
            {
                TotalResults = 1,
                Groups = new List<Group>()
                {
                    new Group()
                    {
                        ID = "Fake ID",
                        DisplayName = "Fake group name",
                         Members = new List<UserGroupMember>()
                        {
                            new UserGroupMember()
                            {
                                Display = "Fake user name",
                                Value = "Fake ID User"
                            }
                        }
                    }
                }
            })).Verifiable();

            return db;
        }
    }
}
