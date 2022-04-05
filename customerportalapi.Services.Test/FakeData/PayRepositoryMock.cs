using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Services.Test.FakeData
{
    public static class PayRepositoryMock
    {

        public static Mock<IPayRepository> PayRepository()
        {
            var db = new Mock<IPayRepository>();
            db.Setup(x => x.GetByExternalId(It.IsAny<string>())).Returns(new Pay()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = "00",
                Message = "fake message",
                SmContractCode = "fakesmcontractCode",
                Username = "fake username",
                DocumentId = "fake documentId",
                InvoiceNumber = "fake invoiceNumber"
            }).Verifiable();

            db.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new Pay()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = "00",
                Message = "fake message",
                SmContractCode = "fakesmcontractCode",
                Username = "fake username",
                DocumentId = "fake documentId",
                InvoiceNumber = "fake invoiceNumber"
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<Pay>())).Returns(new Pay()
            {   
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = "00",
                Message = "fake message",
                SmContractCode = "fakesmcontractCode",
                Username = "fake username",
                DocumentId = "fake documentId",
                InvoiceNumber = "fake invoiceNumber"
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<Pay>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<Pay>())).Returns(Task.FromResult(true)).Verifiable();


            return db;
        }
    }
}
