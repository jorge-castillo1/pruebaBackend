using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test.FakeData
{
    public static class DocumentRepositoryMock
    {
        public static Mock<IDocumentRepository> DocumentRepository()
        {
            var db = new Mock<IDocumentRepository>();
            // List<DocumentMetadata> Search(DocumentMetadataSearchFilter filter);
            db.Setup(x => x.Search(It.IsAny<DocumentMetadataSearchFilter>())).Returns(Task.FromResult(new List<DocumentMetadata>())).Verifiable();

            return db;
        }
    }
}
