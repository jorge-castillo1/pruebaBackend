using System;
using System.Threading.Tasks;
using System.Net.Http;
using customerportalapi.Entities;
using System.Collections.Generic;

namespace customerportalapi.Repositories.interfaces
{
    public interface ISignatureRepository
    {
        Task<Guid> CreateSignature(MultipartFormDataContent form);
        Task<bool> CancelSignature(string id);
        Task<List<SignatureProcess>> SearchSignaturesAsync(SignatureSearchFilter filter);
    }
}
