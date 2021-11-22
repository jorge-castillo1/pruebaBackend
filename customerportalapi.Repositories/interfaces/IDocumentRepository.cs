using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IDocumentRepository
    {
        Task<List<DocumentMetadata>> Search(DocumentMetadataSearchFilter filter);

        Task<string> SaveDocumentAsync(Document document);

        Task<string> SaveDocumentBlobStorageUnitImageContainerAsync(Document document);

        Task<string> SaveDocumentBlobStorageStoreFacadeImageContainerAsync(Document document);

        Task<string> DeleteDocumentBlobStorageStoreFacadeImageContainerAsync(string path);

        Task<string> GetDocumentAsync(string documentid);

        Task<Document> GetFullDocumentAsync(string documentid);

        Task<BlobResult> GetDocumentBlobStorageUnitImageAsync(string name);

        Task<BlobResult> GetDocumentBlobStorageStoreFacadeImageAsync(string name);
    }
}
