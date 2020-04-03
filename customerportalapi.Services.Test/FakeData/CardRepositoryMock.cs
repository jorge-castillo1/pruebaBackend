using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Services.Test.FakeData
{
    public static class CardRepositoryMock
    {

        public static Mock<ICardRepository> CardRepository()
        {
            var db = new Mock<ICardRepository>();
            db.Setup(x => x.GetByExternalId(It.IsAny<string>())).Returns(new Card()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = 00,
                Message = "fake message",
                Cardholder = "fakecardholder",
                Expirydate = "fake expiryDate",
                Typecard = "fake typeCard",
                Cardnumber = "fake cardNumber",
                ContractNumber = "fake cntractNumber",
                Username = "fake username",
                Current = false,
                DocumentId = "fake documentId"
            }).Verifiable();

            db.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new Card()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = 00,
                Message = "fake message",
                Cardholder = "fakecardholder",
                Expirydate = "fake expiryDate",
                Typecard = "fake typeCard",
                Cardnumber = "fake cardNumber",
                ContractNumber = "fake cntractNumber",
                Username = "fake username",
                Current = false,
                DocumentId = "fake documentId"
            }).Verifiable();

            db.Setup(x => x.Update(It.IsAny<Card>())).Returns(new Card()
            {   
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                ExternalId = "12345678A",
                Idcustomer = "fakeIdCustomer",
                Siteid = "fake siteId",
                Token = "fake token",
                Status = 00,
                Message = "fake message",
                Cardholder = "fakecardholder",
                Expirydate = "fake expiryDate",
                Typecard = "fake typeCard",
                Cardnumber = "fake cardNumber",
                ContractNumber = "fake cntractNumber",
                Username = "fake username",
                Current = false,
                DocumentId = "fake documentId"
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<Card>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<Card>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Find(It.IsAny<CardSearchFilter>())).Returns(
                new List<Card>(){
                    new Card()
                    {   
                        Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                        ExternalId = "12345678A",
                        Idcustomer = "fakeIdCustomer",
                        Siteid = "fake siteId",
                        Token = "fake token",
                        Status = 00,
                        Message = "fake message",
                        Cardholder = "fakecardholder",
                        Expirydate = "fake expiryDate",
                        Typecard = "fake typeCard",
                        Cardnumber = "fake cardNumber",
                        ContractNumber = "fake cntractNumber",
                        Username = "fake username",
                        Current = false,
                        DocumentId = "fake documentId"
                    }
                }
            ).Verifiable();

            return db;
        }
    }
}
