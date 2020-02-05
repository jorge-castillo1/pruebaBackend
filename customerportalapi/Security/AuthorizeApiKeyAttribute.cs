using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace customerportalapi.Security
{
    public class AuthorizeApiKeyAttribute : TypeFilterAttribute
    {
        public AuthorizeApiKeyAttribute() : base(typeof(AuthorizeApiKeyFilter))
        {

        }
    }
}
