using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class EkomiWidgetRepository : IEkomiWidgetRepository
    {
        private readonly IMongoCollectionWrapper<EkomiWidget> _ekomiWidgets;

        public EkomiWidgetRepository(IConfiguration config, IMongoCollectionWrapper<EkomiWidget> ekomiWidgets)
        {
            _ekomiWidgets = ekomiWidgets;
        }

        public EkomiWidget Get(string siteId, string ekomiLanguage)
        {
            EkomiWidget ekomiWidget = new EkomiWidget();

            var ekomiWidgetsInfo = _ekomiWidgets.FindOne(t => t.SiteId == siteId && t.EkomiLanguage == ekomiLanguage);
            foreach (var e in ekomiWidgetsInfo)
            {
                ekomiWidget = e;
            }
            return ekomiWidget;
        }

        public EkomiWidget GetById(string id)
        {
            EkomiWidget ekomiWidget = new EkomiWidget();

            List<EkomiWidget> ekomiWidgetsInfo = _ekomiWidgets.FindOne(t => t.Id == id);
            foreach (var e in ekomiWidgetsInfo)
            {
                ekomiWidget = e;
            }
            return ekomiWidget;
        }

        public Task<bool> Create(EkomiWidget ekomiWidget)
        {
            _ekomiWidgets.InsertOne(ekomiWidget);
            return Task.FromResult(true);
        }

        public Task<bool> CreateMultiple(List<EkomiWidget> ekomiWidgets)
        {

            foreach (var e in ekomiWidgets)
            {
                _ekomiWidgets.InsertOne(e);
            } 
            return Task.FromResult(true);
        }

        public EkomiWidget Update(EkomiWidget ekomiWidget)
        {
            //update
            var filter = Builders<EkomiWidget>.Filter.Eq(s => s.Id, ekomiWidget.Id);
            var result = _ekomiWidgets.ReplaceOne(filter, ekomiWidget);

            return ekomiWidget;
        }

         public Task<bool> Delete(string id)
        {
            //update Card
            var filter = Builders<EkomiWidget>.Filter.Eq("_id", id);
            _ekomiWidgets.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public List<EkomiWidget> Find(EkomiWidgetSearchFilter filter)
        {
            FilterDefinition<EkomiWidget> filters = Builders<EkomiWidget>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.SiteId))
                filters = filters & Builders<EkomiWidget>.Filter.Eq(x => x.SiteId, filter.SiteId);

            if (!string.IsNullOrEmpty(filter.EkomiLanguage))
                filters = filters & Builders<EkomiWidget>.Filter.Eq(x => x.EkomiLanguage, filter.EkomiLanguage);

            if (!string.IsNullOrEmpty(filter.EkomiWidgetTokens))
                filters = filters & Builders<EkomiWidget>.Filter.Eq(x => x.EkomiWidgetTokens, filter.EkomiWidgetTokens);

            if (!string.IsNullOrEmpty(filter.EkomiCustomerId))
                filters = filters & Builders<EkomiWidget>.Filter.Eq(x => x.EkomiCustomerId, filter.EkomiCustomerId);

            return _ekomiWidgets.Find(filters, 1, 0);
        }
    }
}
