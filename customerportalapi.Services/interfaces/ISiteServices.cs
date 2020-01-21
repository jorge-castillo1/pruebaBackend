using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface ISiteServices
    {
        Task<List<Site>> GetContractsAsync(string dni);
        Task<List<Store>> GetStoresAsync(string country, string city);
        Task<List<Country>> GetStoresCountriesAsync();
        Task<List<City>> GetStoresCitiesAsync(string country);
    }
}
