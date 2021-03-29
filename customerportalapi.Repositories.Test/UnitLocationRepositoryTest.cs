using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class UnitLocationRepositoryTest
    {
        IConfigurationRoot _configurations;
        Mock<IMongoCollectionWrapper<UnitLocation>> _sizeCodes;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();
            _sizeCodes = new Mock<IMongoCollectionWrapper<UnitLocation>>();
            _sizeCodes.Setup(x => x.FindOne(It.IsAny<Expression<Func<UnitLocation, bool>>>(), It.IsAny<FindOptions>())).Returns(
                new List<UnitLocation>() {
                    new UnitLocation() {
                        SiteCode = "Fake-SiteCode",
                        SizeCode = "Fake-SizeCode",
                        Description = "Fake Description"
                    },
                    new UnitLocation() {
                        SiteCode = "Fake-SiteCode2",
                        SizeCode = "Fake-SizeCode2",
                        Description = "Fake Description2"
                    }
                }
            );

            _sizeCodes.Setup(x => x.InsertOne(It.IsAny<UnitLocation>())).Verifiable();
            _sizeCodes.Setup(x => x.ReplaceOne(It.IsAny<FilterDefinition<UnitLocation>>(), It.IsAny<UnitLocation>())).Returns(new Mock<ReplaceOneResult>().Object).Verifiable();
            _sizeCodes.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<UnitLocation>>())).Returns(Task.FromResult(new Mock<DeleteResult>().Object)).Verifiable();
        }

        [TestMethod]
        public void AlRecuperarUnUnitLocation_BySizeCode_NoSeProducenErrores()
        {
            //Arrange
            UnitLocation location = new UnitLocation();
            string sizeCode = "Fake-SizeCode";
            //Act
            UnitLocationRepository _sizeCodeRepository = new UnitLocationRepository(_configurations, _sizeCodes.Object);
            location = _sizeCodeRepository.GetBySizeCode(sizeCode);

            //Assert
            Assert.AreEqual(location.SizeCode, sizeCode);
        }

        [TestMethod]
        public void AlRecuperarUnUnitLocation_Filter_NoSeProducenErrores()
        {
            //Arrange
            List<UnitLocation> locations = new List<UnitLocation>();

            string siteCode = "Fake-SiteCode";
            string sizeCode = "Fake-SizeCode";

            //Act
            UnitLocationRepository _sizeCodeRepository = new UnitLocationRepository(_configurations, _sizeCodes.Object);
            UnitLocationSearchFilter filter = new UnitLocationSearchFilter()
            {
                SiteCode = "Fake-SiteCode",
                SizeCode = "Fake-SiseCode",
                Description = "Fake-Description"
            };
            locations = _sizeCodeRepository.Find(filter);

            //Assert
            Assert.AreEqual(locations.Count, 1);
            Assert.AreEqual(locations[0].SiteCode, siteCode);
            Assert.AreEqual(locations[0].SizeCode, sizeCode);
        }
    }
}
