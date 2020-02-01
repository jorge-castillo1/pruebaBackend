using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customerportalapi.Security
{
    public class AuthorizeTokenAttribute : TypeFilterAttribute
    {
        //public AuthorizeTokenAttribute(IHostingEnvironment environment, IConfiguration config, ILog log) : base(typeof(AuthorizeTokenFilter))
        public AuthorizeTokenAttribute() : base(typeof(AuthorizeTokenFilter))
        {

        }

    }
}
