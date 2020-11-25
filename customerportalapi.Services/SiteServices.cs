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
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _config;
        private readonly IMailRepository _mailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;




        public SiteServices(
            IUserRepository userRepository,
            IContractRepository contractRepository,
            IStoreRepository storeRepository,
            IDistributedCache distributedCache,
            IIdentityRepository identityRepository,
            IContractSMRepository contractSMRepository,
            IConfiguration config,
            IMailRepository mailRepository,
            IEmailTemplateRepository emailTemplateRepository
        )
        {
            _userRepository = userRepository;
            _contractRepository = contractRepository;
            _storeRepository = storeRepository;
            _distributedCache = distributedCache;
            _identityRepository = identityRepository;
            _contractSMRepository = contractSMRepository;
            _config = config;
            _mailRepository = mailRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }


        public async Task<List<Site>> GetContractsAsync(string username)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUser(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //2. If exist complete data from external repository
            //Invoke repository
            string accountType = (user.Usertype == (int)UserTypes.Business) ? AccountType.Business : AccountType.Residential;
            List<Contract> entitylist = await _contractRepository.GetContractsAsync(user.Dni, accountType);

            List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => new
            {
                Name = x.StoreData.StoreName,
                x.StoreData.Telephone,
                x.StoreData.CoordinatesLatitude,
                x.StoreData.CoordinatesLongitude,
                x.StoreData.EmailAddress1,
                x.StoreData.StoreCode,
                x.StoreData.StoreId
            }))
            {
                string storeId = storegroup.Key.StoreId.ToString();
                Site site = new Site
                {
                    Name = storegroup.Key.Name,
                    Telephone = storegroup.Key.Telephone,
                    CoordinatesLatitude = storegroup.Key.CoordinatesLatitude,
                    CoordinatesLongitude = storegroup.Key.CoordinatesLongitude,
                    EmailAddress1 = storegroup.Key.EmailAddress1,
                    StoreCode = storegroup.Key.StoreCode,
                    StoreId = storeId
                };

                foreach (var contract in storegroup)
                {
                    //ToDo: remove this and clean contract entity
                    contract.StoreCode = contract.StoreData.StoreCode;
                    contract.AccessType = contract.StoreData.AccessType;
                    contract.MapLink = contract.StoreData.MapLink;

                    SMContract contractSM = await _contractSMRepository.GetAccessCodeAsync(contract.SmContractCode);
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

        public async Task<Paginate<Store>> GetPaginatedStoresAsync(string countryCode, string city, int skip, int limit)
        {
            List<Store> storeList = await GetStoresAsync(countryCode, city);
            Paginate<Store> result = new Paginate<Store>
            {
                Total = storeList.Count,
                List = storeList.Skip(skip).Take(limit).ToList(),
                Skip = skip,
                Limit = limit
            };
            return result;
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

        public async Task<bool> IsAccessCodeAvailableAsync()
        {
            var user = Thread.CurrentPrincipal;
            User loginUser = _userRepository.GetCurrentUser(user.Identity.Name);

            //Check bad access code attempts and validate timestamp to allow try again
            if (DateTime.Now.ToUniversalTime().AddMinutes(Int32.Parse(_config["AccessCodeUnblockedTime"]) * -1) > loginUser.LastAccessCodeAttempts)
                return true;

            if (loginUser.AccessCodeAttempts < Int32.Parse(_config["AccessCodeMaxAttempts"]))
                return true;
            else
                return false;
        }

        public async Task<AccessCode> GetAccessCodeAsync(string contractId, string password) {

            var user = Thread.CurrentPrincipal;
            User loginUser = _userRepository.GetCurrentUser(user.Identity.Name);

            try
            {
                Token token = await _identityRepository.Authorize(new Login()
                {
                    Username = user.Identity.Name,
                    Password = password
                });

                if (token.AccesToken == null)
                {
                    throw new ServiceException("Password not valid", HttpStatusCode.BadRequest);
                }

                //Initialize invalid attempt
                loginUser.AccessCodeAttempts = 0;
                loginUser.LastAccessCodeAttempts = DateTime.Now.ToUniversalTime();
                _userRepository.Update(loginUser);

                AccessCode entity = new AccessCode();
                entity.AccesToken = token.AccesToken;
                entity.RefreshToken = token.RefreshToken;
                entity.IdToken = token.IdToken;
                entity.TokenType = token.TokenType;
                entity.ExpiresIn = token.ExpiresIn;
                entity.Scope = token.Scope;

                Contract contract = await _contractRepository.GetContractAsync(contractId);
                SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(contract.SmContractCode);

                entity.Password = smContract.Password;
                entity.ContractId = contractId;

                return entity;
            }
            catch (Exception ex)
            {

                //Accumulate invalid attempt
                loginUser.AccessCodeAttempts = loginUser.AccessCodeAttempts + 1;
                loginUser.LastAccessCodeAttempts = DateTime.Now.ToUniversalTime();
                _userRepository.Update(loginUser);

                throw ex;
            }
        }

        public async Task<Unit> GetUnitAsync(Guid id)
        {
            return await _storeRepository.GetUnitAsync(id);
        }

        public async Task<Unit> GetUnitBySMIdAsync(string smid)
        {
            return await _storeRepository.GetUnitBySMIdAsync(smid);
        }

        public async Task<List<SiteInvoices>> GetLastInvoices(string username)
        {
            List<SiteInvoices> siteInvoices = new List<SiteInvoices>();
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUser(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, "Dni", "Not exist");

            //2. If exist complete data from external repository
            //Invoke repository
            string accountType = (user.Usertype == (int)UserTypes.Business) ? AccountType.Business : AccountType.Residential;
            List<Contract> entitylist = await _contractRepository.GetContractsAsync(user.Dni, accountType);

            //3. From one contract get customer invoces
            if (entitylist.Count == 0)
                return siteInvoices;

            string contractNumber = entitylist[0].SmContractCode;
            List<Invoice> invoices = new List<Invoice>();
            invoices = await _contractSMRepository.GetInvoicesAsync(contractNumber);

            //4. Only returns 3 invoices for every Site
            List<Invoice> filteredInvoices = new List<Invoice>();
            var sites = invoices.GroupBy(x => x.SiteID);
            foreach (var sitegroup in sites)
            {
                var orderGroup = sitegroup.OrderByDescending(x => x.DocumentDate);
                var num = 0;
                foreach (var orderedinvoice in orderGroup)
                {
                    if (num < 3 || orderedinvoice.OutStanding > 0 || orderedinvoice.OutStanding < 0)
                    {
                        filteredInvoices.Add(orderedinvoice);
                    }

                    num++;
                }
            }

            //5. Complete siteContracts with filteredInvoices
            //List<Site> stores = new List<Site>();
            foreach (var storegroup in entitylist.GroupBy(x => new
            {
                Name = x.StoreData.StoreName,
                x.StoreData.StoreCode,
                x.StoreData.StoreId
            }))
            {
                string storeId = storegroup.Key.StoreId.ToString();
                SiteInvoices site = new SiteInvoices
                {
                    Name = storegroup.Key.Name,
                    StoreCode = storegroup.Key.StoreCode,
                    StoreId = storeId
                };

                site.Invoices.AddRange(filteredInvoices.FindAll(x => x.SiteID == site.StoreCode));
                siteInvoices.Add(site);
            }

            return siteInvoices;
        }

        public async Task<bool> UpdateAccessCodeAsync(string contractId, string password) {
            var user = Thread.CurrentPrincipal;
            User currentUser = _userRepository.GetCurrentUser(user.Identity.Name);
            // 1. GetContract to get subcontract
            SMContract smContract = await _contractSMRepository.GetAccessCodeAsync(contractId);

             if (smContract.Contractnumber == null)
                throw new ServiceException("Contract does not exist.", HttpStatusCode.NotFound, "contractId", "Not exist");

            // 2. Get subcontract SM
            SubContract subContract = await _contractSMRepository.GetSubContractAsync(contractId, smContract.Unitid);

            if (subContract.SubContractId == null)
                throw new ServiceException("SubContractId does not exist.", HttpStatusCode.NotFound, "SubContractId", "Not exist");

            if (password == smContract.Password)
                throw new ServiceException("Security access code error", HttpStatusCode.BadRequest);

            // 3. Update  password in SM
            UpdateAccessCode updateAccessCode = new UpdateAccessCode {
                SubContractId = subContract.SubContractId,
                UnitId = smContract.Unitid,
                Code = password
            };

            bool updateAccCode = await _contractSMRepository.UpdateAccessCodeAsync(updateAccessCode);

            if (updateAccCode == false)
                throw new ServiceException("Error updating accessCode.", HttpStatusCode.InternalServerError);



            EmailTemplate editDataCustomerTemplate = _emailTemplateRepository.getTemplate((int)EmailTemplateTypes.EditAccessCode, currentUser.Language);

            if (editDataCustomerTemplate._id != null)
            {
                Email message = new Email();
                message.To.Add(currentUser.Email);
                message.Subject = editDataCustomerTemplate.subject;
                string htmlbody = editDataCustomerTemplate.body.Replace("{", "{{").Replace("}", "}}").Replace("%{{", "{").Replace("}}%", "}");
                message.Body = string.Format(htmlbody, currentUser.Name);
                await _mailRepository.Send(message);
            }

            return updateAccCode;



        }

    }
}
