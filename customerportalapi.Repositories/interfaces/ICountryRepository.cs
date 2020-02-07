using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.interfaces
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetCountriesAsync();
    }
}
