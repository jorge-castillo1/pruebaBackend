using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IEkomiWidgetRepository
    {
        EkomiWidget Get(string siteId, string ekomiLanguage);

        EkomiWidget GetById(string id);

        Task<bool> Create(EkomiWidget ekomiWidget);

        Task<bool> CreateMultiple(List<EkomiWidget> ekomiWidgets);

        EkomiWidget Update(EkomiWidget ejomiWidget);

        Task<bool> Delete(string id);
        
        List<EkomiWidget> Find(EkomiWidgetSearchFilter filter);
    }
}
