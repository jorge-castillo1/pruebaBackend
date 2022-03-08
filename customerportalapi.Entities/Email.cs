using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Email
    {
        public List<string> To { get; set; } = new List<string>() { };
        public List<string> Cc { get; set; } = new List<string>() { };
        public List<string> Cco { get; set; } = new List<string>() { };
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailFlow { get; set; } = string.Empty;
    }
}
