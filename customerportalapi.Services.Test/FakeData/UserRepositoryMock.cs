using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class UserRepositoryMock
    {
        public static Mock<IUserRepository> InvalidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUser(It.IsAny<string>())).Returns(new Entities.User()
            {
                dni = "12345678A",
                email = "fake email",
                language = "fake lang",
                profilepicture = "fake profile image",
                emailverified = false,
                usertype = 1
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUser(It.IsAny<string>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1",
                language = "fake lang",
                profilepicture = "fake profile image",
                emailverified = true,
                usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1 modified",
                language = "fake lang modified",
                profilepicture = "fake profile image modified",
                emailverified = true,
                usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Valid_InActiveUser_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUser(It.IsAny<string>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1",
                language = "fake lang",
                profilepicture = "fake profile image",
                emailverified = false,
                usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1 modified",
                language = "fake lang modified",
                profilepicture = "fake profile image modified",
                emailverified = false,
                usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Valid_InActiveUserByToken_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetUserByInvitationToken(It.IsAny<string>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1",
                language = "fake lang",
                profilepicture = "fake profile image",
                emailverified = false,
                invitationtoken = "8e8b9c6c-8943-4482-891d-b92d7414d283",
                usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "fake email 1",
                language = "fake lang",
                profilepicture = "fake profile image",
                emailverified = true,
                invitationtoken = string.Empty,
                usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Invalid_ActiveUserByToken_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetUserByInvitationToken(It.IsAny<string>())).Returns(new User()).Verifiable();

            return db;
        }
    }
}
