using System;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using customerportalapi.Repositories.Interfaces;
using Moq;

namespace customerportalapi.Services.Test.FakeData
{
    public static class UserRepositoryMock
    {
        public static Mock<IUserRepository> InvalidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Dni = "12345678A",
                Email = "fake email",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10)
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByEmail(It.IsAny<string>())).Returns(new User()).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Dni = "12345678A",
                Email = "fake email",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                LastEmailSent = EmailTemplateTypes.WelcomeEmailExtended.ToString()
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30)
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByEmail(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30)
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30)
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "en",
                Profilepicture = "fake profile image modified",
                Emailverified = true,
                Usertype = 1,
                LoginAttempts = 1,
                LastLoginAttempts = DateTime.Now.ToUniversalTime(),
                AccessCodeAttempts = 1,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime()
            }).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository_With5Attempts()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30)
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30)
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "en",
                Profilepicture = "fake profile image modified",
                Emailverified = true,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime(),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime()
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> InvalidUserRepository_With5Attempts()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10)
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10)
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "en",
                Profilepicture = "fake profile image modified",
                Emailverified = true,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime()
            }).Verifiable();

            return db;
        }
        public static Mock<IUserRepository> Valid_InActiveUser_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByEmail(It.IsAny<string>())).Returns(new User()).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1,
                LastLoginAttempts = DateTime.Now.ToUniversalTime()
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "en",
                Profilepicture = "fake profile image modified",
                Emailverified = false,
                Usertype = 1
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> Valid_InActiveUserByToken_Repository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetUserByInvitationToken(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Invitationtoken = "8e8b9c6c-8943-4482-891d-b92d7414d283",
                Usertype = 1,
                Username = "fake.user",
                Password = "1234"
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Invitationtoken = string.Empty,
                Usertype = 1,
                Username = "fake.user",
                Password = "1234"
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

        public static Mock<IUserRepository> ValidUserRepository_ByEmail()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Dni = "12345678A",
                Email = "fake email",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                Username = "fake.user",
                Password = "1234"
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByEmail(It.IsAny<string>())).Returns(new User
            {
                Id = "id",
                Dni = "12345678A",
                Email = "fake email",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                Username = "fake.user",
                Password = "1234"
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Dni = "12345678A",
                Email = "fake email",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = false,
                Usertype = 1,
                LoginAttempts = 5,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 5,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                Username = "fake.user",
                Password = "1234"
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository_With_ExternalId()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.GetCurrentUserByUsername(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30),
                ExternalId = "ExternalId"
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByEmail(It.IsAny<string>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30),
                ExternalId = "ExternalId"
            }).Verifiable();

            db.Setup(x => x.GetCurrentUserByDniAndType(It.IsAny<string>(), It.IsAny<int>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1",
                Name = "fake name",
                Language = "en",
                Profilepicture = "fake profile image",
                Emailverified = true,
                Usertype = 1,
                Username = "fake username",
                LoginAttempts = 0,
                LastLoginAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-10),
                AccessCodeAttempts = 0,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime().AddMinutes(-30),
                ExternalId = "ExternalId"
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<User>())).Returns(new User
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                Dni = "12345678A",
                Email = "fake email 1 modified",
                Language = "en",
                Profilepicture = "fake profile image modified",
                Emailverified = true,
                Usertype = 1,
                LoginAttempts = 1,
                LastLoginAttempts = DateTime.Now.ToUniversalTime(),
                AccessCodeAttempts = 1,
                LastAccessCodeAttempts = DateTime.Now.ToUniversalTime(),
                ExternalId = "ExternalId",
                Username = "fake.user",
                Password = "1234"
            }).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<User>())).Returns(Task.FromResult(true)).Verifiable();

            return db;
        }
    }
}
