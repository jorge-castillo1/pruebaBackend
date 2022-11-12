﻿using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class EkomiwidgetServiceTest
    {
        private Mock<IEkomiWidgetRepository> _ekomiWidgetRepository;

        [TestInitialize]
        public void Setup()
        {
            _ekomiWidgetRepository = EkomiWidgetRepositoryMock.EkomiWidgetRepository();
        }

        [TestMethod]
        public void GetEkomiwidget_returns_ekomiWidget()
        {
            string StoreCode = "fake siteId";
            string language = "fake language";

            _ekomiWidgetRepository = EkomiWidgetRepositoryMock.EkomiWidgetRepository();

            EkomiWidgetService service = new EkomiWidgetService(_ekomiWidgetRepository.Object);
            var result = service.GetEkomiWidget(StoreCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(StoreCode, result.StoreCode);
            _ekomiWidgetRepository.Verify(x => x.Get(It.IsAny<string>()));
        }

        [TestMethod]
        public void CreateEkomiWidget_returns_task_bool()
        {
            EkomiWidget ekomiWidget = new EkomiWidget()
            {
                EkomiCustomerId = "fake customerId",
                EkomiWidgetTokens = "fake ekomiwidgetTokens",
                StoreCode = "fake siteId0"
            };

            var mockRepository = new Mock<IEkomiWidgetRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(new EkomiWidget()
            {
                Id = null,
                EkomiCustomerId = null,
              
                EkomiWidgetTokens = null,
                StoreCode = null
            }).Verifiable();
            mockRepository.Setup(x => x.Create(It.IsAny<EkomiWidget>())).Returns(Task.FromResult(true)).Verifiable();

            EkomiWidgetService service = new EkomiWidgetService(mockRepository.Object);
            var result = service.CreateEkomiWidget(ekomiWidget);

            Assert.IsNotNull(result);
            mockRepository.Verify(x => x.Create(It.IsAny<EkomiWidget>()));
        }

        [TestMethod]
        public void CreateMultipleEkomiWidget_returns_task_bool()
        {
            List<EkomiWidget> ekList = new List<EkomiWidget>(){

                new EkomiWidget()
                {
                    EkomiCustomerId = "fake customerId",
                   
                    EkomiWidgetTokens = "fake ekomiwidgetTokens",
                    StoreCode = "fake siteId1"
                }
            };

            var mockRepository = new Mock<IEkomiWidgetRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<string>())).Returns(new EkomiWidget()
            {
                Id = null,
                EkomiCustomerId = null,
                EkomiWidgetTokens = null,
                StoreCode = null
            }).Verifiable();

            mockRepository.Setup(x => x.CreateMultiple(It.IsAny<List<EkomiWidget>>())).Returns(Task.FromResult(true)).Verifiable();

            EkomiWidgetService service = new EkomiWidgetService(mockRepository.Object);
            var result = service.CreateMultipleEkomiWidgets(ekList);

            Assert.IsNotNull(result);
            mockRepository.Verify(x => x.CreateMultiple(It.IsAny<List<EkomiWidget>>()));
        }

        [TestMethod]
        public void UpdateEkomiWidget_returns_ekomiWidget()
        {
            EkomiWidget ekomiWidget = new EkomiWidget()
            {
                Id = "b02fc244-40e4-e511-80bf-00155d018a4g",
                EkomiCustomerId = "fake customerId",
               
                EkomiWidgetTokens = "fake ekomiwidgetTokens",
                StoreCode = "fake siteId"
            };


            _ekomiWidgetRepository = EkomiWidgetRepositoryMock.EkomiWidgetRepository();

            EkomiWidgetService service = new EkomiWidgetService(_ekomiWidgetRepository.Object);
            var result = service.UpdateEkomiWidget(ekomiWidget);

            Assert.IsNotNull(result);
            _ekomiWidgetRepository.Verify(x => x.Update(It.IsAny<EkomiWidget>()));
        }

        [TestMethod]
        public void DeleteEkomiWidget_returns_task_bool()
        {
            string id = "b02fc244-40e4-e511-80bf-00155d018a4g";

            _ekomiWidgetRepository = EkomiWidgetRepositoryMock.EkomiWidgetRepository();

            EkomiWidgetService service = new EkomiWidgetService(_ekomiWidgetRepository.Object);
            var result = service.DeleteEkomiWidget(id);

            Assert.IsNotNull(result);
            _ekomiWidgetRepository.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [TestMethod]
        public void FindEkomiWidget_returns_ekomiWidgetList()
        {
            EkomiWidgetSearchFilter ekomiWidgetSearchFilter = new  EkomiWidgetSearchFilter()
            {
                EkomiCustomerId = "fake customerId"
            };


            _ekomiWidgetRepository = EkomiWidgetRepositoryMock.EkomiWidgetRepository();

            EkomiWidgetService service = new EkomiWidgetService(_ekomiWidgetRepository.Object);
            var result = service.FindEkomiWidgets(ekomiWidgetSearchFilter);

            Assert.IsNotNull(result);
            _ekomiWidgetRepository.Verify(x => x.Find(It.IsAny<EkomiWidgetSearchFilter>()));
        }

    }
}
