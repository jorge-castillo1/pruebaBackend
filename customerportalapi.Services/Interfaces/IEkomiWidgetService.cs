using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface IEkomiWidgetService
    {
        EkomiWidget GetEkomiWidget(string storeCode);

        Task<bool> CreateEkomiWidget(EkomiWidget ekomiWidget);

        Task<bool> CreateMultipleEkomiWidgets(List<EkomiWidget> ekomiWidgets);

        EkomiWidget UpdateEkomiWidget(EkomiWidget ekomiWidget);

        Task<bool> DeleteEkomiWidget(string id);

        List<EkomiWidget> FindEkomiWidgets(EkomiWidgetSearchFilter ekomiWidgetSearchFilter);
    }
}
