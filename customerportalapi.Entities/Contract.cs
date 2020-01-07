using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Contract
    {
        public string ContractNumber { get; set; }

        public string Store { get; set; }

        public string ContractDate { get; set; }

        public int ContractStatus { get; set; }
    }
}
