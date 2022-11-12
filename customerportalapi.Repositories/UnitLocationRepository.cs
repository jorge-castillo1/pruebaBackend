using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class UnitLocationRepository : IUnitLocationRepository
    {
        private readonly IMongoCollectionWrapper<UnitLocation> _sizeCodes;

        public UnitLocationRepository(IConfiguration config, IMongoCollectionWrapper<UnitLocation> sizeCodes)
        {
            _sizeCodes = sizeCodes;
        }

        public UnitLocation GetBySizeCode(string sizeCode)
        {
            UnitLocation location = new UnitLocation();

            var sizeCodesInfo = _sizeCodes.FindOne(t => t.SizeCode == sizeCode);
            foreach (var c in sizeCodesInfo)
            {
                location = c;
            }
            return location;
        }

        public UnitLocation Update(UnitLocation location)
        {
            var filter = Builders<UnitLocation>.Filter.Eq(s => s.SiteCode, location.SiteCode);
            filter &= Builders<UnitLocation>.Filter.Eq(s => s.SizeCode, location.SizeCode);
            var result = _sizeCodes.ReplaceOne(filter, location);

            return location;
        }

        public Task<bool> Create(UnitLocation location)
        {
            _sizeCodes.InsertOne(location);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(UnitLocation location)
        {
            var filter = Builders<UnitLocation>.Filter.Eq("SiteCode", location.SizeCode);
            filter &= Builders<UnitLocation>.Filter.Eq("SizeCode", location.SizeCode);
            _sizeCodes.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public List<UnitLocation> Find(UnitLocationSearchFilter filter)
        {
            FilterDefinition<UnitLocation> filters = Builders<UnitLocation>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.SiteCode))
                filters &= Builders<UnitLocation>.Filter.Eq(x => x.SiteCode, filter.SiteCode);

            if (!string.IsNullOrEmpty(filter.SizeCode))
                filters &= Builders<UnitLocation>.Filter.Eq(x => x.SizeCode, filter.SizeCode);

            if (!string.IsNullOrEmpty(filter.Description))
                filters &= Builders<UnitLocation>.Filter.Eq(x => x.Description, filter.Description);

            return _sizeCodes.Find(filters, 1, 0);
        }
    }
}
