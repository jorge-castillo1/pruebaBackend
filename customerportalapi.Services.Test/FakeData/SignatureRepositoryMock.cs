using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace customerportalapi.Services.Test.FakeData
{
    public static class SignatureRepositoryMock
    {
        public static Mock<ISignatureRepository> SignatureRepository()
        {
            var db = new Mock<ISignatureRepository>();
            db.Setup(x => x.CreateSignature(It.IsAny<MultipartFormDataContent>())).Returns(Task.FromResult(new Guid())).Verifiable();

            return db;
        }
    }
}
