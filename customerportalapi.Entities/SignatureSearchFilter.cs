﻿namespace customerportalapi.Entities
{
    public class SignatureSearchFilter
    {
        public SignatureSearchFilterData Filters { get; set; } = new SignatureSearchFilterData();
        public SignatureSearchFilterPagination Pagination { get; set; } = new SignatureSearchFilterPagination();
    }
}
