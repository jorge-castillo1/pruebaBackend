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

        Task<string> SaveBlobAsync(Document document);

        Task<string> GetDocumentAsync(string documentid);
    }
}
