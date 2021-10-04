using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IDocumentRepository _documentRepository;
        private readonly IStoreImageRepository _storeImageRepository;

        public SiteServices(
            IUserRepository userRepository,
            IContractRepository contractRepository,
            IStoreRepository storeRepository,
            IDistributedCache distributedCache,
            IIdentityRepository identityRepository,
            IContractSMRepository contractSMRepository,
            IConfiguration config,
            IMailRepository mailRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IDocumentRepository documentRepository,
            IStoreImageRepository storeImageRepository
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
            _documentRepository = documentRepository;
            _storeImageRepository = storeImageRepository;
        }


        public async Task<List<Site>> GetContractsAsync(string username)
        {
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUserByUsername(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.NotExist);

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
                x.StoreData.StoreId,
                x.StoreData.StoreImage
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
                    SMContract contractSM = null;
                    if (contract != null && contract.Unit != null && !string.IsNullOrEmpty(contract.SmContractCode))
                        contractSM = await _contractSMRepository.GetAccessCodeAsync(contract.SmContractCode);

                    // only active contracts, if the contract has "terminated", the field "Leaving" have information.
                    if (contractSM != null && string.IsNullOrEmpty(contractSM.Leaving))
                    {
                        //ToDo: remove this and clean contract entity
                        contract.StoreCode = contract.StoreData.StoreCode;
                        contract.AccessType = contract.StoreData.AccessType;
                        contract.MapLink = contract.StoreData.MapLink;

                        contract.TimeZone = contractSM.Timezone;
                        contract.StoreData = null;
                        site.Contracts.Add(contract);
                        contract.StoreImage = storegroup.Key.StoreImage;
                    }
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
            User loginUser = _userRepository.GetCurrentUserByUsername(user.Identity.Name);

            //Check bad access code attempts and validate timestamp to allow try again
            if (DateTime.Now.ToUniversalTime().AddMinutes(Int32.Parse(_config["AccessCodeUnblockedTime"]) * -1) > loginUser.LastAccessCodeAttempts)
                return true;

            if (loginUser.AccessCodeAttempts < Int32.Parse(_config["AccessCodeMaxAttempts"]))
                return true;
            else
                return false;
        }

        public async Task<AccessCode> GetAccessCodeAsync(string contractId, string password)
        {

            var user = Thread.CurrentPrincipal;
            User loginUser = _userRepository.GetCurrentUserByUsername(user.Identity.Name);

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

        public async Task<string> SaveImageUnitCategoryAsync(Document document)
        {
            return await _documentRepository.SaveDocumentBlobStorageUnitImageContainerAsync(document);
        }

        public async Task<bool> SaveImageStoreFacadeAsync(Document document, string storeCode)
        {
            //Buscar en BD si existe la imagen
            var storeImageList = _storeImageRepository.Find(new StoreImageSearchFilter() { StoreCode = storeCode });

            if (storeImageList == null || !storeImageList.Any())
            {
                // si no existe se crea un documento con un nombre generado x guid nuevo
                var guid = Guid.NewGuid().ToString();
                document.FileName = Path.ChangeExtension(guid, document.FileExtension);

                var storeImage = new StoreImage
                {
                    Id = null,
                    StoreCode = storeCode,
                    ContainerId = document.FileName
                };

                // se graba el documento en BD
                bool resultDB = await _storeImageRepository.Create(storeImage);
                // se sube el documento con el nuevo nombre
                string documentUrl = await _documentRepository.SaveDocumentBlobStorageStoreFacadeImageContainerAsync(document);

                //Llamada al CRM 

                StoreImageUrl storeImageUrl = new StoreImageUrl
                {
                    StoreCode = storeImage.StoreCode,
                    DocumentUrl = documentUrl
                };

                Store updatedStore = await _storeRepository.UpdateSiteImage(storeImageUrl);
               
                return resultDB &&
                    !string.IsNullOrEmpty(documentUrl) &&
                    !string.IsNullOrEmpty(updatedStore.StoreCode);
            }
            else
            {
                var storeImage = storeImageList.FirstOrDefault();

                // se obtiene el guid a partir del nombre del documento
                string file = Path.GetFileNameWithoutExtension(storeImage.ContainerId);
                // se le agrega la posible nueva extensión
                document.FileName = Path.ChangeExtension(file, document.FileExtension);

                // se graba en BD el documento con el nuevo nombre
                storeImage.ContainerId = document.FileName;
                var resultDB = _storeImageRepository.Update(storeImage);

                // se borra el antiguo y se sube el nuevo documento
                string resultDelete = await _documentRepository.DeleteDocumentBlobStorageStoreFacadeImageContainerAsync(storeImage.ContainerId);
                string documentUrl = await _documentRepository.SaveDocumentBlobStorageStoreFacadeImageContainerAsync(document);

                //Llamada al CRM 
                StoreImageUrl storeImageUrl = new StoreImageUrl
                {
                    StoreCode = storeImage.StoreCode,
                    DocumentUrl = documentUrl
                };

                Store updatedStore = await _storeRepository.UpdateSiteImage(storeImageUrl);

                return resultDB != null &&
                    !string.IsNullOrEmpty(resultDB.Id) &&
                    !string.IsNullOrEmpty(documentUrl) && 
                    !string.IsNullOrEmpty(updatedStore.StoreCode);
            }
        }

        public async Task<bool> DeleteImageStoreFacadeAsync(string storeCode)
        {
            var storeImageList = _storeImageRepository.Find(new StoreImageSearchFilter() { StoreCode = storeCode });
            if (storeImageList != null && storeImageList.Any())
            {
                var storeImage = storeImageList.FirstOrDefault();

                string resultDelete = await _documentRepository.DeleteDocumentBlobStorageStoreFacadeImageContainerAsync(storeImage.ContainerId);
                bool resultDB = await _storeImageRepository.DeleteByStoreCode(storeCode);

                StoreImageUrl storeImageUrl = new StoreImageUrl
                {
                    StoreCode = storeCode,
                    DocumentUrl = null
                };

                Store updatedStore = await _storeRepository.UpdateSiteImage(storeImageUrl);
                return !string.IsNullOrEmpty(resultDelete) &&
                  resultDB &&
                  !string.IsNullOrEmpty(updatedStore.StoreCode);
            }

            return true;
        }

        public async Task<List<BlobResult>> GetDocumentInfoBlobStorageUnitCategoryImageAsync(string names)
        {
            List<BlobResult> res = new List<BlobResult>();
            if (!string.IsNullOrEmpty(names))
            {
                var files = names.Split(",");

                foreach (var file in files)
                {
                    var result = await _documentRepository.GetDocumentBlobStorageUnitImageAsync(file);
                    if (result != null && !string.IsNullOrEmpty(result.LocalPath))
                    {
                        result.Name = file;
                        res.Add(result);
                    }
                }
            }

            return res;
        }

        public async Task<List<BlobResult>> GetDocumentInfoStoreFacadeAsync(string storeCode)
        {
            List<BlobResult> res = new List<BlobResult>();

            var storeImage = _storeImageRepository.Get(storeCode);
            if (storeImage != null && !string.IsNullOrEmpty(storeImage.ContainerId))
            {
                var result = await _documentRepository.GetDocumentBlobStorageStoreFacadeImageAsync(storeImage.ContainerId);
                if (result != null && !string.IsNullOrEmpty(result.LocalPath))
                {
                    result.Name = storeImage.ContainerId;
                    res.Add(result);
                }
            }

            return res;
        }

        public async Task<List<SiteInvoices>> GetLastInvoices(string username, string contractNumber = null)
        {
            int limitInvoices = 3;
            List<SiteInvoices> siteInvoices = new List<SiteInvoices>();
            //Add customer portal Business Logic
            User user = _userRepository.GetCurrentUserByUsername(username);
            if (user.Id == null)
                throw new ServiceException("User does not exist.", HttpStatusCode.NotFound, FieldNames.Username, ValidationMessages.NotExist);

            //2. If exist complete data from external repository
            //Invoke repository
            string accountType = (user.Usertype == (int)UserTypes.Business) ? AccountType.Business : AccountType.Residential;
            List<Contract> contracts = await _contractRepository.GetContractsAsync(user.Dni, accountType);

            if (!string.IsNullOrEmpty(contractNumber))
                contracts = contracts.Where(c => c.ContractNumber == contractNumber).ToList();

            //3. From contracts get customer invoices
            if (contracts == null || contracts.Count == 0)
                return siteInvoices;

            //4. Group contract by Store
            string previousStoreId = "";
            foreach (var storegroup in contracts.GroupBy(x => new
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

                List<ContractInvoices> contractInvoices = new List<ContractInvoices>();
                List<Invoice> invoicesByCustomerId = new List<Invoice>();
                List<Invoice> invoicesByCustomerIdOrdered = new List<Invoice>();
                foreach (var contract in storegroup)
                {
                    SMContract contractSM = null;
                    if (contract != null && contract.Unit != null && !string.IsNullOrEmpty(contract.SmContractCode))
                        contractSM = await _contractSMRepository.GetAccessCodeAsync(contract.SmContractCode);

                    // only active contracts, if the contract has "terminated", the field "Leaving" have information.
                    if (contractSM != null && string.IsNullOrEmpty(contractSM.Leaving))
                    {
                        List<Invoice> invoicesFiltered = new List<Invoice>();

                        ContractInvoices contractInvoice = new ContractInvoices
                        {
                            ContractId = contract.ContractId,
                            ContractNumber = contract.ContractNumber,
                            SmContractCode = contract.SmContractCode,
                            StoreCode = contract.StoreData.StoreCode
                        };

                        // Only get invoices if previousStoreId is different about current Site / StoreId
                        // GetInvoicesAsync use SmContractCode for get CustomerID, CustomerId is unique by Site / StoreId
                        if (previousStoreId != site.StoreId)
                        {
                            previousStoreId = site.StoreId;
                            invoicesByCustomerId = await _contractSMRepository.GetInvoicesByCustomerIdAsync(contractSM.Customerid);
                            invoicesByCustomerIdOrdered.AddRange(invoicesByCustomerId.OrderByDescending(x => x.DocumentDate));
                        }

                        // First, find unpaid invoices, invoice.OutStanding != 0
                        GetInvoicesWhitOutStanding(contract.Unit.UnitName, invoicesByCustomerIdOrdered, invoicesFiltered);

                        // Second, if invoices by contract is minor to  limitInvoices(3 by default), find invoices by DocumentDate
                        if (invoicesFiltered.Count < limitInvoices && invoicesByCustomerIdOrdered.Count > 0)
                        {
                            GetInvoicesByDocumentDate(limitInvoices, contract.Unit.UnitName, invoicesByCustomerIdOrdered, invoicesFiltered);
                        }

                        contractInvoice.Invoices.AddRange(invoicesFiltered);
                        contractInvoices.Add(contractInvoice);
                    }
                }

                site.Contracts.AddRange(contractInvoices);
                siteInvoices.Add(site);
            }

            return siteInvoices;
        }

        public async Task<bool> UpdateAccessCodeAsync(string contractId, string password)
        {
            var user = Thread.CurrentPrincipal;
            User currentUser = _userRepository.GetCurrentUserByUsername(user.Identity.Name);
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
            UpdateAccessCode updateAccessCode = new UpdateAccessCode
            {
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

        private string GetUnitName(string unitDescription)
        {
            string unitName = !string.IsNullOrEmpty(unitDescription) ? unitDescription.Split(':')[0] : null;

            return unitName;
        }

        private void GetInvoicesWhitOutStanding(string unitName, List<Invoice> invoicesByCustomerId, List<Invoice> invoicesFiltered)
        {

            foreach (Invoice invoice in invoicesByCustomerId)
            {
                string InvoiceUnitName = GetUnitName(invoice.UnitDescription);
                bool contains = invoicesFiltered.Contains(invoice);
                if (!string.IsNullOrEmpty(InvoiceUnitName) && InvoiceUnitName == unitName && !contains && invoice.OutStanding != 0)
                {
                    invoicesFiltered.Add(invoice);
                }
            }
        }

        private void GetInvoicesByDocumentDate(int limitInvoices, string unitName, List<Invoice> invoicesByCustomerId, List<Invoice> invoicesFiltered)
        {
            int pos = 0;
            int num = invoicesFiltered.Count;

            while (num < limitInvoices)
            {
                Invoice invoice = invoicesByCustomerId[pos];
                string invoiceUnitName = GetUnitName(invoice.UnitDescription);
                bool contains = invoicesFiltered.Contains(invoice);
                if (!string.IsNullOrEmpty(invoiceUnitName) && invoiceUnitName == unitName && !contains && num < limitInvoices)
                {
                    invoicesFiltered.Add(invoice);
                    num++;
                }

                if (pos == (invoicesByCustomerId.Count - 1))
                    num = limitInvoices;

                pos++;
            }
        }

    }
}
