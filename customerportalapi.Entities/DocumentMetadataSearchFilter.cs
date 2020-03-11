﻿using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class DocumentMetadataSearchFilter
    {
        public string DocumentId { get; set; }
        public int? DocumentType { get; set; }
        public string StoreName { get; set; }
        public string AccountDni { get; set; }
        public int? AccountType { get; set; }
        public string ContractNumber { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
