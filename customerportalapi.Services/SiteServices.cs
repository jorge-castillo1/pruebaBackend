using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using customerportalapi.Services.Exceptions;
using customerportalapi.Entities.enums;
using System.Threading;

namespace customerportalapi.Services
{
    public class SiteServices : ISiteServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IIdentityRepository _identityRepository;
        private readonly IContractSMRepository _contractSMRepository;


        public SiteServices(IUserRepository userRepository, IContractRepository contractRepository,
            IStoreRepository storeRepository, IDistributedCache distributedCache, IIdentityRepository identityRepository,
            IContractSMRepository contractSMRepository)
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
            _storeRepository = storeRepository;
            _distributedCache = distributedCache;
            _identityRepository = identityRepository;
            _contractSMRepository = contractSMRepository;
        }


        public async Task<List<Site>> GetContractsAsync(string dni, string accountType)
        {
            //Add customer portal Business Logic
            int userType = UserUtils.GetUserType(accountType);
            User user = _userRepository.GetCurrentUserByDniAndType(dni, userType);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //2. If exist complete data from external repository
            //Invoke repository
            List<Contract> entitylist = await _contractRepository.GetContractsAsync(dni, accountType);

            List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => new
            {
                Name = x.StoreData.StoreName,
                x.StoreData.Telephone,
                x.StoreData.CoordinatesLatitude,
                x.StoreData.CoordinatesLongitude,
                x.StoreData.EmailAddress1,
                x.StoreData.StoreCode
            }))
            {
                Site site = new Site
                {
                    Name = storegroup.Key.Name,
                    Telephone = storegroup.Key.Telephone,
                    CoordinatesLatitude = storegroup.Key.CoordinatesLatitude,
                    CoordinatesLongitude = storegroup.Key.CoordinatesLongitude,
                    EmailAddress1 = storegroup.Key.EmailAddress1,
                    StoreCode = storegroup.Key.StoreCode
                };

                foreach (var contract in storegroup)
                {
                    //ToDo: remove this and clean contract entity
                    contract.StoreCode = contract.StoreData.StoreCode;
                    contract.AccessType = contract.StoreData.AccessType;
                    contract.MapLink = contract.StoreData.MapLink;
                    SMContract contractSM = await _contractSMRepository.GetAccessCodeAsync(contract.ContractNumber);
                    contract.TimeZone = contractSM.Timezone;
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

        public async Task<AccessCode> GetAccessCodeAsync(string contractId, string password) {
            // TODO:
            var user = Thread.CurrentPrincipal;
            Token token = await _identityRepository.Authorize(new Login()
            {
                Username = user.Identity.Name,
                Password = password
            });
            AccessCode entity = new AccessCode();
            entity.AccesToken = token.AccesToken;
            entity.RefreshToken = token.RefreshToken;
            entity.IdToken = token.IdToken;
            entity.TokenType = token.TokenType;
            entity.ExpiresIn = token.ExpiresIn;
            entity.Scope = token.Scope;

            if (entity.AccesToken == null) {
                throw new ServiceException("Password not valid", HttpStatusCode.BadRequest);
            }

            SMContract contract = await _contractSMRepository.GetAccessCodeAsync(contractId);

            entity.Password = contract.Password;
            entity.ContractId = contractId;

            return entity;
        }

        public async Task<Unit> GetUnitAsync(Guid id)
        {
            return await _storeRepository.GetUnitAsync(id);
        }

        public async Task<Unit> GetUnitBySMIdAsync(string smid)
        {
            return await _storeRepository.GetUnitBySMIdAsync(smid);
        }
    }
}
