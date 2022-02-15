using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Email
    {
        private List<string> _to = new List<string>();
        private List<string> _cc = new List<string>();
        private List<string> _cco = new List<string>();

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

        public List<string> Cco
        {
            get { return _cco; }
            set { _cco = value; }
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailFlow { get; set; }

    }
}
