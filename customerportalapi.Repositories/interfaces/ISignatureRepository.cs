using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface ISignatureRepository
    {
        Task<Guid> CreateSignature(MultipartFormDataContent form);

        Task<bool> CancelSignature(string id);

        Task<List<SignatureProcess>> SearchSignaturesAsync(SignatureSearchFilter filter);

        Task<List<SignatureResultData>> GetSignatureInfoAsync(string contractNumber, string fromDate,
            string documentCountry, string status = null);

        Task<SignatureResultData> GetSignatureInfoByIdAsync(string signatureId);

        Task<string> UploadDocumentAsync(DocumentMetadata metadata, string documentCountry, string since,
            string status);

        Task<bool> Create(SignatureResultData signatureResult);
    }
}