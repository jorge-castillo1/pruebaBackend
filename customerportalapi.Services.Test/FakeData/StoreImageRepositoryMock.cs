using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Services.Test.FakeData
{
    public static class StoreImageRepositoryMock
    {

        public static Mock<IStoreImageRepository> StoreImageRepository()
        {
            var db = new Mock<IStoreImageRepository>();
            db.Setup(x => x.Get(It.IsAny<string>())).Returns(new StoreImage()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                StoreCode = "BA0V0XXX010620090000",
                ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
            }).Verifiable();

            db.Setup(x => x.GetById(It.IsAny<string>())).Returns(new StoreImage()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                StoreCode = "BA0V0XXX010620090000",
                ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<StoreImage>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Update(It.IsAny<StoreImage>())).Returns(new StoreImage()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                StoreCode = "BA0V0XXX010620090000",
                ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
            }).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<string>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.DeleteByStoreCode(It.IsAny<string>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Find(It.IsAny<StoreImageSearchFilter>())).Returns(
                new List<StoreImage>(){
                    new StoreImage()
                    {
                        Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                        StoreCode = "BA0V0XXX010620090000",
                        ContainerId = "8c305ea4-dc6e-436b-9182-995c4d19ecd9.png"
                    }
                }
            ).Verifiable();

            return db;
        }
    }
}
