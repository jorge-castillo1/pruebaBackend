using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.interfaces
{
    public interface ISiteServices
    {
        Task<List<Site>> GetContractsAsync(string username);
        Task<List<Store>> GetStoresAsync(string countryCode, string city);
        Task<Paginate<Store>> GetPaginatedStoresAsync(string countryCode, string city, int skip, int limit);
        Task<List<Country>> GetStoresCountriesAsync();
        Task<List<City>> GetStoresCitiesAsync(string countryCode);
        Task<Store> GetStoreAsync(string storeCode);
        Task<AccessCode> GetAccessCodeAsync(string contractId, string password);
        Task<Unit> GetUnitAsync(Guid id);
        Task<Unit> GetUnitBySMIdAsync(string smid);
        Task<List<SiteInvoices>> GetLastInvoices(string username);
    }
}
