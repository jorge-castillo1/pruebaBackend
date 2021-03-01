using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class ApsRequest
    {
        public string Dni { get; set; }
        public string ContractNumber { get; set; }
        public string IBAN { get; set; }
    }
}
