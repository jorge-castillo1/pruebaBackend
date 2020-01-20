using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IStoreRepository
    {
       Task<List<Store>> GetStoresAsync();
    }
}
