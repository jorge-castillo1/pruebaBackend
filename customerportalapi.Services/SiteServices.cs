using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using customerportalapi.Services.Exceptions;

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
            User user = _userRepository.GetCurrentUser(dni);
            if (user._id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");
            
            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = await _contractRepository.GetContractsAsync(dni);

            List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => new
            {
                Name = x.StoreData.StoreName,
                x.StoreData.Telephone,
                x.StoreData.CoordinatesLatitude,
                x.StoreData.CoordinatesLongitude
            }))
            {
                Site site = new Site
                {
                    Name = storegroup.Key.Name,
                    Telephone = storegroup.Key.Telephone,
                    CoordinatesLatitude = storegroup.Key.CoordinatesLatitude,
                    CoordinatesLongitude = storegroup.Key.CoordinatesLongitude
                };

                foreach (var contract in storegroup)
                {
                    //ToDo: remove this and clean contract entity
                    contract.StoreData = null;
                    site.Contracts.Add(contract);
                }

                stores.Add(site);
            }

            return stores;
        }

        public async Task<List<Store>> GetStoresAsync(string countryCode, string city)
        {
            List<Store> entitylist = await GetList();

            if (!string.IsNullOrEmpty(countryCode))
                entitylist = entitylist.Where(d => d.CountryCode == countryCode).ToList();

            if (!string.IsNullOrEmpty(city))
                entitylist = entitylist.Where(d => d.City == city).ToList();

            return new List<Store>(entitylist.OrderBy(o => o.Country).ThenBy(o => o.City).ThenBy(o => o.StoreName));
        }

        public async Task<List<Country>> GetStoresCountriesAsync()
        {
            List<Store> entitylist = await GetList();

            var groupedOrdered = entitylist.GroupBy(f => new { f.CountryCode, f.Country })
                .OrderBy(o => o.Key.Country);

            return groupedOrdered.Select(countryGroup => new Country
            {
                Code = countryGroup.Key.CountryCode,
                Name = countryGroup.Key.Country
            }).ToList();
        }

        public async Task<List<City>> GetStoresCitiesAsync(string countryCode)
        {
            List<Store> entitylist = await GetList();

            if (!string.IsNullOrEmpty(countryCode))
                entitylist = entitylist.Where(d => d.CountryCode == countryCode).ToList();

            var groupedOrdered = entitylist.GroupBy(f => f.City).OrderBy(o => o.Key);

            return groupedOrdered.Select(cityGroup => new City { Name = cityGroup.Key }).ToList();
        }

        public async Task<Store> GetStoreAsync(string storeCode)
        {
            return await _storeRepository.GetStoreAsync(storeCode);
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
