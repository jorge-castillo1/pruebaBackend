using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface ISiteServices
    {
        Task<List<Site>> GetContractsAsync(string username);
        Task<List<Store>> GetStoresAsync(string countryCode, string city);
        Task<Paginate<Store>> GetPaginatedStoresAsync(string countryCode, string city, int skip, int limit);
        Task<List<Country>> GetStoresCountriesAsync();
        Task<List<City>> GetStoresCitiesAsync(string countryCode);
        Task<Store> GetStoreAsync(string storeCode);
        Task<bool> IsAccessCodeAvailableAsync();
        Task<AccessCode> GetAccessCodeAsync(string contractId, string password);
        Task<Unit> GetUnitAsync(Guid id);
        Task<Unit> GetUnitBySMIdAsync(string smid);
        Task<string> SaveImageUnitCategoryAsync(Document document);
        Task<bool> SaveImageStoreFacadeAsync(Document document, string storeCode);
        Task<bool> DeleteImageStoreFacadeAsync(string storeCode);
        Task<List<BlobResult>> GetDocumentInfoBlobStorageUnitCategoryImageAsync(string name);
        Task<List<BlobResult>> GetDocumentInfoStoreFacadeAsync(string storeCode);
        Task<List<SiteInvoices>> GetLastInvoices(string username, string contractNumber = null);
        Task<bool> UpdateAccessCodeAsync(string contractId, string black);
    }
}
