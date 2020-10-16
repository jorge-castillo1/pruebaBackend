using System;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
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
                }
            })).Verifiable();

            return db;
        }

    }
}
