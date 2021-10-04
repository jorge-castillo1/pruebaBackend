using System;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IStoreRepository
    {
        Task<List<Store>> GetStoresAsync();
        Task<Store> GetStoreAsync(string storeId);
        Task<Unit> GetUnitAsync(Guid id);
        Task<Unit> GetUnitBySMIdAsync(string smid);
        Task<Store> UpdateSiteImage(StoreImageUrl storeImageUrl);
    }
}
