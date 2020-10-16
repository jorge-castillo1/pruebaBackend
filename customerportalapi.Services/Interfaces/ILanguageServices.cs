using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface ILanguageServices
    {
        Task<List<Language>> GetLanguagesAsync();
    }
}