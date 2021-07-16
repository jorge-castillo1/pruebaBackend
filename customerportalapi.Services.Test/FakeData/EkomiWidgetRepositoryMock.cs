using System.Runtime.CompilerServices;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace customerportalapi.Services.Test.FakeData
{
    public static class EkomiWidgetRepositoryMock
    {

        public static Mock<IEkomiWidgetRepository> EkomiWidgetRepository()
        {
            var db = new Mock<IEkomiWidgetRepository>();
            db.Setup(x => x.Get(It.IsAny<string>())).Returns(new EkomiWidget()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                EkomiCustomerId = "fake customerId",
                EkomiWidgetTokens = "fake ekomiwidgetTokens",
                StoreCode = "fake siteId"
            }).Verifiable();

            db.Setup(x => x.GetById(It.IsAny<string>())).Returns(new EkomiWidget()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                EkomiCustomerId = "fake customerId",
                EkomiWidgetTokens = "fake ekomiwidgetTokens",
                StoreCode = "fake siteId"
            }).Verifiable();

            db.Setup(x => x.Create(It.IsAny<EkomiWidget>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.CreateMultiple(It.IsAny<List<EkomiWidget>>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Update(It.IsAny<EkomiWidget>())).Returns(new EkomiWidget()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                EkomiCustomerId = "fake customerId",
                EkomiWidgetTokens = "fake ekomiwidgetTokens",
                StoreCode = "fake siteId"
            }).Verifiable();

            db.Setup(x => x.Delete(It.IsAny<string>())).Returns(Task.FromResult(true)).Verifiable();

            db.Setup(x => x.Find(It.IsAny<EkomiWidgetSearchFilter>())).Returns(
                new List<EkomiWidget>(){
                    new EkomiWidget()
                    {   
                        Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                        EkomiCustomerId = "fake customerId",
                        EkomiWidgetTokens = "fake ekomiwidgetTokens",
                        StoreCode = "fake siteId"
                    }
                }
            ).Verifiable();

            return db;
        }
    }
}
