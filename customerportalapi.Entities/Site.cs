using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Site
    {
        public string Name { get; set; }
        public List<Contract> Contracts { get; } = new List<Contract>();
    }
}
