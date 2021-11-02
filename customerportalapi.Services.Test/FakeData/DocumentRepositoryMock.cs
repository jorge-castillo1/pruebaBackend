using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;

namespace customerportalapi.Services.Test.FakeData
{
    public static class DocumentRepositoryMock
    {
        public static Mock<IDocumentRepository> DocumentRepository()
        {
            var db = new Mock<IDocumentRepository>();
            db.Setup(x => x.Search(It.IsAny<DocumentMetadataSearchFilter>())).Returns(Task.FromResult(new List<DocumentMetadata>() {
                new DocumentMetadata()
            })).Verifiable();
            db.Setup(x => x.SaveDocumentAsync(It.IsAny<Document>())).Returns(Task.FromResult(Guid.NewGuid().ToString())).Verifiable();
            db.Setup(x => x.GetDocumentAsync(It.IsAny<string>())).Returns(Task.FromResult("base64document")).Verifiable();

            return db;
        }

        public static Mock<IDocumentRepository> NoContractnumberDocumentRepository()
        {
            var db = new Mock<IDocumentRepository>();
            db.Setup(x => x.Search(It.IsAny<DocumentMetadataSearchFilter>())).Returns(Task.FromResult(new List<DocumentMetadata>())).Verifiable();
            db.Setup(x => x.SaveDocumentAsync(It.IsAny<Document>())).Returns(Task.FromResult(Guid.NewGuid().ToString())).Verifiable();
            db.Setup(x => x.GetDocumentAsync(It.IsAny<string>())).Returns(Task.FromResult("base64document")).Verifiable();

            return db;
        }

        public static Mock<IDocumentRepository> MultipleContractsRepository()
        {
            var db = new Mock<IDocumentRepository>();
            db.Setup(x => x.Search(It.IsAny<DocumentMetadataSearchFilter>())).Returns(Task.FromResult(
                new List<DocumentMetadata>()
                {
                    new DocumentMetadata()
                    {
                        DocumentId = "DocumentId",
                        DocumentType = (int)DocumentTypes.Contract,
                        StoreName = "StoreName",
                        AccountDni = "AccountDni",
                        AccountType = (int)UserTypes.Residential,
                        ContractNumber = "ContractNumber",
                        SmContractCode = "SmContractCode",
                        CreatedBy = "CreatedBy",
                        CreationDate = new DateTime(),
                        RelativeUrl = "RelativeUrl",
                        BankAccountOrderNumber = "BankAccountOrderNumber",
                        BankAccountName = "BankAccountName",
                    },
                    new DocumentMetadata()
                    {
                        DocumentId = "DocumentId2",
                        DocumentType = (int)DocumentTypes.Contract,
                        StoreName = "StoreName",
                        AccountDni = "AccountDni",
                        AccountType = (int)UserTypes.Residential,
                        ContractNumber = "ContractNumber",
                        SmContractCode = "SmContractCode",
                        CreatedBy = "CreatedBy",
                        CreationDate = new DateTime(),
                        RelativeUrl = "RelativeUrl",
                        BankAccountOrderNumber = "BankAccountOrderNumber",
                        BankAccountName = "BankAccountName",
                    }
                }
                )).Verifiable();
            db.Setup(x => x.SaveDocumentAsync(It.IsAny<Document>())).Returns(Task.FromResult(Guid.NewGuid().ToString())).Verifiable();
            db.Setup(x => x.GetDocumentAsync(It.IsAny<string>())).Returns(Task.FromResult("base64document")).Verifiable();

            return db;
        }

        public static Mock<IDocumentRepository> MultipleInvoiceRepository()
        {
            var db = new Mock<IDocumentRepository>();
            db.Setup(x => x.Search(It.IsAny<DocumentMetadataSearchFilter>())).Returns(Task.FromResult(
                new List<DocumentMetadata>()
                {
                    new DocumentMetadata()
                    {
                        DocumentId = "DocumentId",
                        DocumentType = (int)DocumentTypes.Invoice,
                        StoreName = "StoreName",
                        AccountDni = "AccountDni",
                        AccountType = (int)UserTypes.Residential,
                        ContractNumber = "ContractNumber",
                        SmContractCode = "SmContractCode",
                        CreatedBy = "CreatedBy",
                        CreationDate = new DateTime(),
                        RelativeUrl = "RelativeUrl",
                        BankAccountOrderNumber = "BankAccountOrderNumber",
                        BankAccountName = "BankAccountName",
                    },
                    new DocumentMetadata()
                    {
                        DocumentId = "DocumentId2",
                        DocumentType = (int)DocumentTypes.Invoice,
                        StoreName = "StoreName",
                        AccountDni = "AccountDni",
                        AccountType = (int)UserTypes.Residential,
                        ContractNumber = "ContractNumber",
                        SmContractCode = "SmContractCode",
                        CreatedBy = "CreatedBy",
                        CreationDate = new DateTime(),
                        RelativeUrl = "RelativeUrl",
                        BankAccountOrderNumber = "BankAccountOrderNumber",
                        BankAccountName = "BankAccountName",
                    }
                }
                )).Verifiable();
            db.Setup(x => x.SaveDocumentAsync(It.IsAny<Document>())).Returns(Task.FromResult(Guid.NewGuid().ToString())).Verifiable();
            db.Setup(x => x.GetDocumentAsync(It.IsAny<string>())).Returns(Task.FromResult("base64document")).Verifiable();

            return db;
        }
    }
}
