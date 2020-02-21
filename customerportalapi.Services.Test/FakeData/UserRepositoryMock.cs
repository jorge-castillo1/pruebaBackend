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
                Dni = "12345678A",
                Email = "fake email",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new Entities.User()
            {
                Dni = "12345678A",
                Email = "fake email",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUser(It.IsAny<string>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "fake lang modified",
                Profilepicture = "fake profile image modified",
                Emailverified = true,
                Usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Valid_InActiveUser_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUser(It.IsAny<string>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

             db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "fake lang modified",
                Profilepicture = "fake profile image modified",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Valid_InActiveUserByToken_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetUserByInvitationToken(It.IsAny<string>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Invitationtoken = "8e8b9c6c-8943-4482-891d-b92d7414d283",
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "fake lang",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Invitationtoken = string.Empty,
                Usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Invalid_ActiveUserByToken_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetUserByInvitationToken(It.IsAny<string>())).Returns(new User()).Verifiable();
            db.Setup(x => x.GetUserByForgotPasswordToken(It.IsAny<string>())).Returns(new User()).Verifiable();

            return db;
        }
    }
}
