using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Services.Test.FakeData
{
    public static class UserRepositoryMock
    {
        public static Mock<IUserRepository> InvalidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.getCurrentUser(It.IsAny<string>())).Returns(new Entities.User()
            {
                dni = "12345678A",
                email = "user email",
                language = "fake lang",
                profilepicture = "fake profile image"
            }).Verifiable();

            return db;
        }

        public static Mock<IUserRepository> ValidUserRepository()
        {
            var db = new Mock<IUserRepository>();
            db.Setup(x => x.getCurrentUser(It.IsAny<string>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "user email",
                language = "fake lang",
                profilepicture = "fake profile image"
            }).Verifiable();

            db.Setup(x => x.update(It.IsAny<User>())).Returns(new User()
            {
                _id = "b02fc244-40e4-e511-80bf-00155d018a4f",
                dni = "12345678A",
                email = "user email",
                language = "fake lang modified",
                profilepicture = "fake profile image modified"
            }).Verifiable();

            return db;
        }
    }
}
