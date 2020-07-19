using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class Email
    {
        private List<string> _to = new List<string>();
        private List<string> _cc = new List<string>();

        public List<string> To
        {
            get { return _to; }
            set { _to = value; }
        }
        public List<string> Cc
        {
            get { return _cc; }
            set { _cc = value; }
        }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
