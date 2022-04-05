using System;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Services.Test.FakeData
{
    public static class LanguageRepositoryMock
    {
        public static Mock<ILanguageRepository> LanguageRepository()
        {
             var db = new Mock<ILanguageRepository>();
            db.Setup(x => x.GetLanguagesAsync()).Returns( Task.FromResult(new List<Language>()
            {
                new Language()
                {
                    Id = Guid.NewGuid(),
                    Name = "English",
                    IsoCode = "EN",
                    StatusCode = "1"
                },
                new Language()
                {
                    Id = Guid.NewGuid(),
                    Name = "Spanish",
                    IsoCode = "ES",
                    StatusCode = "1"
                },
                new Language()
                {
                    Id = Guid.NewGuid(),
                    Name = "Portuguese",
                    IsoCode = "PT",
                    StatusCode = "1"
                },
                new Language()
                {
                    Id = Guid.NewGuid(),
                    Name = "Français",
                    IsoCode = "FR",
                    StatusCode = "1"
                }
            })).Verifiable();

            return db;
        }

    }
}
