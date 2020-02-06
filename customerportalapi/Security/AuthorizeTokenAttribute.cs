using Microsoft.AspNetCore.Mvc;

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
