using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class ProfileRepositoryMock
    {
        public static Mock<IProfileRepository> ProfileRepository()
        {
            var db = new Mock<IProfileRepository>();
            db.Setup(x => x.GetProfileAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new Profile()
            {
                Fullname = "fake name",
                Address = "fake Address",
                DocumentNumber = "fake Document Number",
                Language = "fake language",
                EmailAddress1 = "fake email 1",
                EmailAddress2 = "fake email 2"
            })).Verifiable();

            db.Setup(x => x.UpdateProfileAsync(It.IsAny<Profile>())).Returns(Task.FromResult(new Profile()
            {
                Fullname = "fake name",
                Address = "fake Address modified",
                DocumentNumber = "fake Document Number",
                Language = "fake language modified",
                EmailAddress1 = "fake email 1 modified",
                EmailAddress2 = "fake email 2 modified"
            })).Verifiable();

            db.Setup(x => x.GetProfilePermissionsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ProfilePermissions()
            {
                DocumentNumber = "fake Document Number",
                CanManageAccounts = true,
                CanManageContacts = false
            })).Verifiable();

            return db;
        }
    }
}
