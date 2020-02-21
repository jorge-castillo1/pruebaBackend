using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class SignatureResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public SignatureResultResponse Result { get; set; } = new SignatureResultResponse();
        
    }

    public class SignatureResultResponse
    {
        public Guid Id { get; set; }
        public List<SignatureResultDocumentResponse> Documents { get; set; } = new List<SignatureResultDocumentResponse>();
    }

    public class SignatureResultDocumentResponse
    {
        public Guid Id { get; set; }
    }
}
