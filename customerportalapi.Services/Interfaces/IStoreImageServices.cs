using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.interfaces
{
    public interface IStoreImageServices
    {
        StoreImage GetStoreImage(string storeCode);

        Task<bool> CreateStoreImage(StoreImage storeImage);

        StoreImage UpdateStoreImage(StoreImage storeImage);

        Task<bool> DeleteStoreImage(string id);

        List<StoreImage> FindStoreImage(StoreImageSearchFilter storeImageSearchFilter);
    }
}