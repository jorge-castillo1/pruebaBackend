using customerportalapi.Entities.enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace customerportalapi.Security
{
    public class AuthorizeAzureADAttribute : TypeFilterAttribute
    {
        public List<RoleGroupTypes> RoleGroups { get; set; }

        public AuthorizeAzureADAttribute(RoleGroupTypes[] roleGroups) : base(typeof(AuthorizeAzureADFilter))
        {
            RoleGroups = new List<RoleGroupTypes>();
            RoleGroups.AddRange(roleGroups);

            Arguments = new object[] { RoleGroups.ToArray() };
        }
    }
}
