using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class SignatureListResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<SignatureResult> Result { get; set; } = new List<SignatureResult>();
    }

    public class SignatureResultDataListResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<SignatureResultData> Result { get; set; } = new List<SignatureResultData>();
    }
}