using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using customerportalapi.Services.Interfaces;

namespace customerportalapi.Services
{
    public class CountryServices : ICountryServices
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IDistributedCache _distributedCache;

        public CountryServices(ICountryRepository countryRepository, IDistributedCache distributedCache)
        {
            _countryRepository = countryRepository;
            _distributedCache = distributedCache;
        }


        public async Task<List<Country>> GetCountriesAsync()
        {
            List<Country> entitylist = await GetList();

            return entitylist.OrderBy(o => o.Name).ToList();
        }

        private async Task<List<Country>> GetList()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));
            DistributedMongoDbCache<List<Country>> distributedCache = new DistributedMongoDbCache<List<Country>>(_distributedCache, cacheEntryOptions);

            return await distributedCache.GetOrCreateCache("Countries", async () => await _countryRepository.GetCountriesAsync());
        }
    }
}
