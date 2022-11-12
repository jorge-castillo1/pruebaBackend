using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class UserAccountRepositoryMock
    {
        public static Mock<IUserAccountRepository> ValidUserRepository()
        {
             var db = new Mock<IUserAccountRepository>();
            db.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(new UserAccount()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Username = "username",
                Profilepicture = "fake profile image",
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<UserAccount>())).Returns(new UserAccount()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Username = "username",
                Profilepicture = "fake profile image modified",
            }).Verifiable();

            return db;
        }

    }
}
