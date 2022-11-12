using System.Collections.Generic;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using customerportalapi.Entities.Enums;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class LanguageServicesTest
    {
        private Mock<ILanguageRepository> _repository;
        public Mock<IConfiguration> Config { get; private set; }

        [TestInitialize]
        public void Setup()
        {
            _repository = LanguageRepositoryMock.LanguageRepository();
            Config = new Mock<IConfiguration>();
        }

        [TestMethod]
        public async Task GetLanguages_returns_language_list()
        {
            //Arrange

            //Act
            LanguageServices service = new LanguageServices(_repository.Object);
            List<Language> languages = await service.GetLanguagesAsync();

            //Assert
            Assert.IsNotNull(languages);
            Assert.AreEqual(4, languages.Count);
        }
    }
}
