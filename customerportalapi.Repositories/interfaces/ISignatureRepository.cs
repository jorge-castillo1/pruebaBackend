using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace customerportalapi.Repositories.interfaces
{
    public interface ISignatureRepository
    {
        Task<Guid> CreateSignature(MultipartFormDataContent form);
    }
}
