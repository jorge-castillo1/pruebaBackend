using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using customerportalapi.Services.Interfaces;

namespace customerportalapi.Services
{
    public class LanguageServices : ILanguageServices
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageServices(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }


        public async Task<List<Language>> GetLanguagesAsync()
        {
            List<Language> entitylist = await _languageRepository.GetLanguagesAsync();

            return entitylist;
        }

    }
}
