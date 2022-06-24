using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
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
                Language = "English",
                EmailAddress1 = "fake email 1",
                EmailAddress2 = "fake email 2",
                Admincontact = false,
                Supercontact = false,
                WebPortalAccess = false,
            })).Verifiable();

            db.Setup(x => x.UpdateProfileAsync(It.IsAny<Profile>())).Returns(Task.FromResult(new Profile()
            {
                Fullname = "fake name",
                Address = "fake Address modified",
                DocumentNumber = "fake Document Number",
                Language = "English",
                EmailAddress1 = "fake email 1 modified",
                EmailAddress2 = "fake email 2 modified",
                Admincontact = false,
                Supercontact = false,
                WebPortalAccess = false,
            })).Verifiable();

            db.Setup(x => x.GetProfilePermissionsAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ProfilePermissions()
            {
                DocumentNumber = "fake Document Number",
                CanManageAccounts = true,
                CanManageContacts = false
            })).Verifiable();

            db.Setup(x => x.GetAccountAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new AccountProfile()
            {
                SmCustomerId = "RAAAAAAAAAAAAA0000"
            })).Verifiable();


            return db;
        }

        public static Mock<IProfileRepository> AccountProfileRepository()
        {
            var db = new Mock<IProfileRepository>();
            db.Setup(x => x.GetAccountAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new AccountProfile()
            {
                SmCustomerId =  "fake SM Customer Id",
                Phone1 = "fake phone 1",
                MobilePhone1 = "fake mobile phone 1",
                EmailAddress1 = "fake email 2 modified",
                EmailAddress2 = "fake email 2 modified"
            })).Verifiable();

            db.Setup(x => x.GetAccountByDocumentNumberAsync(It.IsAny<string>())).Returns(Task.FromResult(new AccountProfile()
            {
                SmCustomerId =  "fake SM Customer Id",
                Phone1 = "fake phone 1",
                MobilePhone1 = "fake mobile phone 1",
                EmailAddress1 = "fake email 2 modified",
                EmailAddress2 = "fake email 2 modified"
            })).Verifiable();

            return db;
        }

        public static Mock<IProfileRepository> AccountProfileRepositoryInvalid()
        {
            var db = new Mock<IProfileRepository>();
            db.Setup(x => x.GetAccountAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new AccountProfile())).Verifiable();

            return db;
        }
    }
}
