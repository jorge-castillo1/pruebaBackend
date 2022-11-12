using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class AccessCode : Token
    {
        public string Password { get; set; }

        public string ContractId { get; set; }
    }
}
