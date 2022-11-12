﻿using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IEkomiWidgetRepository
    {
        EkomiWidget Get(string storeCode);

        EkomiWidget GetById(string id);

        Task<bool> Create(EkomiWidget ekomiWidget);

        Task<bool> CreateMultiple(List<EkomiWidget> ekomiWidgets);

        EkomiWidget Update(EkomiWidget ejomiWidget);

        Task<bool> Delete(string id);
        
        List<EkomiWidget> Find(EkomiWidgetSearchFilter filter);
    }
}
