using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace customerportalapi.Services.Exceptions
{
    public class ServiceException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;

        public string Field { get; }
        public string FieldMessage { get; }

        public ServiceException(string message, HttpStatusCode statuscode, string field = "", string fieldmessage = "") : base(message)
        {
            StatusCode = statuscode;
            Field = field;
            FieldMessage = fieldmessage;
        }
    }
}
