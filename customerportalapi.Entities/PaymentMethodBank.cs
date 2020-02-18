﻿using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class PaymentMethodBank
    {
        public string Dni { get; set; }
        public string AccountType { get; set; }
        public string ContractNumber { get; set; }
        public string IBAN { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
