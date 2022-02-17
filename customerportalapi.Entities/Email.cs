using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Email
    {
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Cco { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailFlow { get; set; }
    }
}
