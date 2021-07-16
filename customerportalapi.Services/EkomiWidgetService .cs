using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using customerportalapi.Services.Interfaces;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Services.Exceptions;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class EkomiWidgetService : IEkomiWidgetService
    {
        private readonly IEkomiWidgetRepository _ekomiWidgetRepository;
        public EkomiWidgetService(IEkomiWidgetRepository ekomiWidgetRepository)
        {
            _ekomiWidgetRepository = ekomiWidgetRepository;
        }

        public EkomiWidget GetEkomiWidget(string storeCode)
        {
            EkomiWidget ekomiWidget = _ekomiWidgetRepository.Get(storeCode);
            return ekomiWidget;
        }

        public Task<bool> CreateEkomiWidget(EkomiWidget ekomiWidget)
        {
            if (string.IsNullOrEmpty(ekomiWidget.StoreCode))
                throw new ServiceException("SiteId required", HttpStatusCode.BadRequest, "SiteId", "SiteId required");

            if (string.IsNullOrEmpty(ekomiWidget.EkomiCustomerId))
                throw new ServiceException("EkomicustomerId required", HttpStatusCode.BadRequest, "EkomicustomerId", "EkomicustomerId required");

            if (string.IsNullOrEmpty(ekomiWidget.EkomiWidgetTokens))
                throw new ServiceException("EkomiWidgetTokens required", HttpStatusCode.BadRequest, "EkomiWidgetTokens", "EkomiWidgetTokens required");

            EkomiWidget findEkomiWidget = _ekomiWidgetRepository.Get(ekomiWidget.StoreCode);

            if (findEkomiWidget.Id != null)
                throw new ServiceException("EkomiWidget exist with same siteId and EkomiLanguage please update", HttpStatusCode.BadRequest, "SiteId - EkomiLanguage", "EkomiWidget exist with same siteId and EkomiLanguage please update");

            return _ekomiWidgetRepository.Create(ekomiWidget);
        }

        public Task<bool> CreateMultipleEkomiWidgets(List<EkomiWidget> ekomiWidgets)
        {
            // Check if exits some ekomiWidget with same SiteId and Language
            EkomiWidget findEkomiWidget;
            foreach( EkomiWidget ekomiWidget in ekomiWidgets)
            {
                findEkomiWidget  = _ekomiWidgetRepository.Get(ekomiWidget.StoreCode);
                if (findEkomiWidget.Id != null)
                    throw new ServiceException("EkomiWidget exist with same siteId and EkomiLanguage please update", HttpStatusCode.BadRequest, "SiteId - EkomiLanguage", "EkomiWidget exist with same siteId and EkomiLanguage please update");
            }

            return _ekomiWidgetRepository.CreateMultiple(ekomiWidgets);
        }

        public EkomiWidget UpdateEkomiWidget(EkomiWidget ekomiWidget)
        {
            
            EkomiWidget findEkomiWidget = _ekomiWidgetRepository.GetById(ekomiWidget.Id);

            if (findEkomiWidget == null)
                throw new ServiceException("EkomiWidget by Id Not Found", HttpStatusCode.NotFound, "Id", "EkomiWidget Id Not Found");

            EkomiWidget ekomiWidgetToUpdate = new EkomiWidget()
            {
                Id = findEkomiWidget.Id,
                StoreCode = ekomiWidget.StoreCode,
                EkomiCustomerId = ekomiWidget.EkomiCustomerId,
                EkomiWidgetTokens = ekomiWidget.EkomiWidgetTokens,
            };

            return _ekomiWidgetRepository.Update(ekomiWidgetToUpdate);

        }

        public Task<bool> DeleteEkomiWidget(string id)
        {
            
            if (string.IsNullOrEmpty(id))
                throw new ServiceException("Id required", HttpStatusCode.BadRequest, "Id", "EkomiWidget Id required");

            return _ekomiWidgetRepository.Delete(id);

        }

        public List<EkomiWidget> FindEkomiWidgets(EkomiWidgetSearchFilter ekomiWidgetSearchFilter)
        {

            return _ekomiWidgetRepository.Find(ekomiWidgetSearchFilter);

        }

    }
}
