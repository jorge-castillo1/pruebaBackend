using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface ICountryServices
    {
        Task<List<Country>> GetCountriesAsync();
    }
}