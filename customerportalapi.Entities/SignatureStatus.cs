using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class SignatureStatus
    {
        public string DocumentId { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
    }
}
