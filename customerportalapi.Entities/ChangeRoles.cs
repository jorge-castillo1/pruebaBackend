using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ChangeRoles
    {
        public string UserName { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }
        public bool Value { get; set; }
    }
}