using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Paginate<T>
    {
        public List<T> List { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
