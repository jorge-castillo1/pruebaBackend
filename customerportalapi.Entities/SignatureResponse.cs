using System;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class SignatureResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public SignatureResult Result { get; set; } = new SignatureResult();
    }

    public class SignatureDataResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public SignatureResultData Result { get; set; } = new SignatureResultData();
    }

    public class SignatureResultData
    {
        public Guid Id { get; set; }
        public string Created_at { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>() { };
        public List<SignatureDocumentResult> Documents { get; set; } = new List<SignatureDocumentResult>();
    }

    public class SignatureResultDataResponse
    {
        public List<SignatureResultData> ListSignatureResultData { get; set; } = new List<SignatureResultData>() { };
        public List<Contract> ListContracts { get; set; } = new List<Contract>() { };
        public List<string> ListContractsNoProcessed { get; set; } = new List<string>() { };
    }

    public class SignatureResult
    {
        public Guid Id { get; set; }
        public string Created_at { get; set; }
        public List<SignatureDocumentResult> Documents { get; set; } = new List<SignatureDocumentResult>();
    }

    public class SignatureDocumentResult
    {
        public Guid Id { get; set; }
        public string Created_at { get; set; }
        public string Email { get; set; }
        public List<EventResult> Events { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public FileResult File { get; set; }
    }

    public class EventResult
    {
        public string Type { get; set; }
        public string Created_at { get; set; }
    }

    public class FileResult
    {
        public string Name { get; set; }
        public int Pages { get; set; }
        public int Size { get; set; }
    }
}