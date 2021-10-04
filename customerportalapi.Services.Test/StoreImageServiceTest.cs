using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class StoreImageServiceTest
    {
        private Mock<IStoreImageRepository> _storeImageRepository;

        [TestInitialize]
        public void Setup()
        {
            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();
        }

        [TestMethod]
        public void GetStoreImage_returns_storeImage()
        {
            string storeCode = "BA0V0XXX010620090000";

            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();

            StoreImageServices service = new StoreImageServices(_storeImageRepository.Object);
            var result = service.GetStoreImage(storeCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(storeCode, result.StoreCode);
            _storeImageRepository.Verify(x => x.Get(It.IsAny<string>()));
        }

        [TestMethod]
        public void CreateStoreImage_returns_task_bool()
        {
            StoreImage storeImage = new StoreImage()
            {
                StoreCode = "BA0V0XXX010620090000",
                ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
            };

            var mockRepository = new Mock<IStoreImageRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(new StoreImage()
            {
                Id = null,
                ContainerId = null,
                StoreCode = null
            }).Verifiable();
            mockRepository.Setup(x => x.Create(It.IsAny<StoreImage>())).Returns(Task.FromResult(true)).Verifiable();

            StoreImageServices service = new StoreImageServices(mockRepository.Object);
            var result = service.CreateStoreImage(storeImage);

            Assert.IsNotNull(result);
            mockRepository.Verify(x => x.Create(It.IsAny<StoreImage>()));
        }

        [TestMethod]
        public void UpdateStoreImage_returns_storeImage()
        {
            StoreImage storeImage = new StoreImage()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                StoreCode = "BA0V0XXX010620090000",
                ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
            };


            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();

            StoreImageServices service = new StoreImageServices(_storeImageRepository.Object);
            var result = service.UpdateStoreImage(storeImage);

            Assert.IsNotNull(result);
            _storeImageRepository.Verify(x => x.Update(It.IsAny<StoreImage>()));
        }

        [TestMethod]
        public void DeleteStoreImage_returns_task_bool()
        {
            string id = "b02fc244-40e4-e511-80bf-00155d018a4g";

            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();

            StoreImageServices service = new StoreImageServices(_storeImageRepository.Object);
            var result = service.DeleteStoreImage(id);

            Assert.IsNotNull(result);
            _storeImageRepository.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [TestMethod]
        public void DeleteStoreImageByStoreCode_returns_task_bool()
        {
            string storeCode = "BA0V0XXX010620090000";

            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();

            StoreImageServices service = new StoreImageServices(_storeImageRepository.Object);
            var result = service.DeleteStoreImage(storeCode);

            Assert.IsNotNull(result);
            _storeImageRepository.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [TestMethod]
        public void FindStoreImage_returns_storeImageList()
        {
            StoreImageSearchFilter storeImageSearchFilter = new StoreImageSearchFilter()
            {
                StoreCode = "BA0V0XXX010620090000"
            };

            _storeImageRepository = StoreImageRepositoryMock.StoreImageRepository();

            StoreImageServices service = new StoreImageServices(_storeImageRepository.Object);
            var result = service.FindStoreImage(storeImageSearchFilter);

            Assert.IsNotNull(result);
            _storeImageRepository.Verify(x => x.Find(It.IsAny<StoreImageSearchFilter>()));
        }

    }
}
