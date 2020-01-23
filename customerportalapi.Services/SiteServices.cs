using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace customerportalapi.Services
{
    public class SiteServices : ISiteServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IDistributedCache _distributedCache;

        public SiteServices(IUserRepository userRepository, IContractRepository contractRepository,
            IStoreRepository storeRepository, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
            _storeRepository = storeRepository;
            _distributedCache = distributedCache;
        }


        public async Task<List<Site>> GetContractsAsync(string dni)
        {
            //Add customer portal Business Logic
            User user = _userRepository.getCurrentUser(dni);
            if (user._id == null)
                throw new ArgumentException("User does not exist.");


            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = await _contractRepository.GetContractsAsync(dni);

            List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => x.Store))
            {
                Site store = new Site { Name = storegroup.Key };
                foreach (var contract in storegroup)
                    store.Contracts.Add(contract);

                stores.Add(store);
            }

            return stores;
        }

        public async Task<List<Store>> GetStoresAsync(string country, string city)
        {
            List<Store> entitylist = await GetList();

            if (!string.IsNullOrEmpty(country))
                entitylist = entitylist.Where(d => d.Country == country).ToList();

            if (!string.IsNullOrEmpty(city))
                entitylist = entitylist.Where(d => d.City == city).ToList();

            return new List<Store>(entitylist.OrderBy(o => o.Country).ThenBy(o => o.City).ThenBy(o => o.StoreName));
        }

        public async Task<List<Country>> GetStoresCountriesAsync()
        {
            List<Store> entitylist = await GetList();

            var groupedOrdered = entitylist.GroupBy(f => f.Country).OrderBy(o => o.Key);

            return groupedOrdered.Select(countryGroup => new Country { Name = countryGroup.Key }).ToList();
        }

        public async Task<List<City>> GetStoresCitiesAsync(string country)
        {
            List<Store> entitylist = await GetList();

            if (!string.IsNullOrEmpty(country))
                entitylist = entitylist.Where(d => d.Country == country).ToList();

            var groupedOrdered = entitylist.GroupBy(f => f.City).OrderBy(o => o.Key);

            return groupedOrdered.Select(cityGroup => new City { Name = cityGroup.Key }).ToList();
        }

        public async Task<Store> GetStoreAsync(string storeId)
        {
            return await _storeRepository.GetStoreAsync(storeId);
        }

        private async Task<List<Store>> GetList()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));
            DistributedMongoDbCache<List<Store>> distributedCache = new DistributedMongoDbCache<List<Store>>(_distributedCache, cacheEntryOptions);

            return await distributedCache.GetOrCreateCache("Store", async () => await _storeRepository.GetStoresAsync());
        }
    }
}
