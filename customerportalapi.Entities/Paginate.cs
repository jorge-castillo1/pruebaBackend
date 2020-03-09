using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Paginate<T>
    {
        public List<T> List { get; set; }
        public int Total { get; set; }
    }
}
