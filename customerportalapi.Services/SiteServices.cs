using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Services.interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace customerportalapi.Services
{
    public class SiteServices : ISiteServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMemoryCache _memoryCache;

        public SiteServices(IUserRepository userRepository, IContractRepository contractRepository, IStoreRepository storeRepository, IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
            _storeRepository = storeRepository;
            _memoryCache = memoryCache;
        }


        public async Task<List<Site>> GetContractsAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(dni);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");


            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = new List<Contract>();
            entitylist = await _contractRepository.GetContractsAsync(dni);

            List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => x.Store))
            {
                Site store = new Site();
                store.Name = storegroup.Key;
                foreach (var contract in storegroup)
                    store.Contracts.Add(contract);

                stores.Add(store);
            }

            return stores;
        }

        public async Task<List<Store>> GetStoresAsync(string country, string city)
        {
            List<Store> entitylist = await GetOrCreateCache();

            if (!string.IsNullOrEmpty(country))
                entitylist = entitylist.Where(d => d.Country == country).ToList();

            if (!string.IsNullOrEmpty(city))
                entitylist = entitylist.Where(d => d.City == city).ToList();

            return new List<Store>(entitylist.OrderBy(o => o.Country).ThenBy(o => o.City).ThenBy(o => o.StoreName));
        }

        public async Task<List<Country>> GetStoresCountriesAsync()
        {
            List<Store> entitylist = await GetOrCreateCache();

            var groupedOrdered = entitylist.GroupBy(f => f.Country).OrderBy(o => o.Key);

            return groupedOrdered.Select(countryGroup => new Country { Name = countryGroup.Key }).ToList();
        }

        public async Task<List<City>> GetStoresCitiesAsync(string country)
        {
            List<Store> entitylist = await GetOrCreateCache();

            if (!string.IsNullOrEmpty(country))
                entitylist = entitylist.Where(d => d.Country == country).ToList();

            var groupedOrdered = entitylist.GroupBy(f => f.City).OrderBy(o => o.Key);

            return groupedOrdered.Select(cityGroup => new City { Name = cityGroup.Key }).ToList();
        }


        private async Task<List<Store>> GetOrCreateCache()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.Entry, out List<Store> entitylist))
            {
                entitylist = await _storeRepository.GetStoresAsync();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Set cache entry size by extension method.
                    .SetSize(1)
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(CacheKeys.Entry, entitylist, cacheEntryOptions);
            }

            return entitylist;
        }
    }
}
