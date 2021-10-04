using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace customerportalapi.Repositories
{
    public class StoreImageRepository : IStoreImageRepository
    {
        private readonly IMongoCollectionWrapper<StoreImage> _storeImages;

        public StoreImageRepository(IConfiguration config, IMongoCollectionWrapper<StoreImage> storeImages)
        {
            _storeImages = storeImages;
        }

        public StoreImage Get(string storeCode)
        {
            return _storeImages.FindOne(t => t.StoreCode == storeCode).FirstOrDefault();
        }

        public StoreImage GetById(string id)
        {
            return _storeImages.FindOne(t => t.Id == id).FirstOrDefault();
        }

        public Task<bool> Create(StoreImage storeImage)
        {
            _storeImages.InsertOne(storeImage);
            return Task.FromResult(true);
        }

        public Task<bool> CreateMultiple(List<StoreImage> storeImages)
        {
            foreach (var e in storeImages)
            {
                _storeImages.InsertOne(e);
            }
            return Task.FromResult(true);
        }

        public StoreImage Update(StoreImage storeImage)
        {         
            var filter = Builders<StoreImage>.Filter.Eq(s => s.StoreCode, storeImage.StoreCode);
            var result = _storeImages.ReplaceOne(filter, storeImage);

            return storeImage;
        }

        public Task<bool> Delete(string id)
        {            
            var filter = Builders<StoreImage>.Filter.Eq("_id", id);
            _storeImages.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteByStoreCode(string storeCode)
        {
            var filter = Builders<StoreImage>.Filter.Eq("storecode", storeCode);
            _storeImages.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public List<StoreImage> Find(StoreImageSearchFilter filter)
        {
            FilterDefinition<StoreImage> filters = Builders<StoreImage>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.StoreCode))
                filters &= Builders<StoreImage>.Filter.Eq(x => x.StoreCode, filter.StoreCode);

            if (!string.IsNullOrEmpty(filter.ContainerId))
                filters &= Builders<StoreImage>.Filter.Eq(x => x.ContainerId, filter.ContainerId);

            return _storeImages.Find(filters, 1, 0);
        }
    }
}
