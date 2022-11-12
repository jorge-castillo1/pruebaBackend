using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IStoreImageRepository
    {
        StoreImage Get(string storeCode);

        StoreImage GetById(string id);

        Task<bool> Create(StoreImage storeImage);

        StoreImage Update(StoreImage storeImage);

        Task<bool> Delete(string id);

        Task<bool> DeleteByStoreCode(string storeCode);

        List<StoreImage> Find(StoreImageSearchFilter filter);
    }
}
