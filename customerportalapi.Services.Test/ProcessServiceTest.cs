﻿using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Test.FakeData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Test
{
    [TestClass]
    public class ProcessServiceTest
    {
        private Mock<IProcessRepository> _processRepository;
        private Mock<ISignatureRepository> _signatureRepository;

        [TestInitialize]
        public void Setup()
        {
            _processRepository = ProcessRepositoryMock.ProcessRepository();
            _signatureRepository = SignatureRepositoryMock.SignatureRepository();
        }

        [TestMethod]
        public void SiSeEncuentranProcesos_DevuelveElMasReciente()
        {
            //Arrange
            string user = "fake user";
            string contractnumber = "fake contract";
            int processtype = 1;

            _processRepository = ProcessRepositoryMock.MoreThanOneResultProcessRepository();
            ProcessService service = new ProcessService(_processRepository.Object, _signatureRepository.Object);
            var result = service.GetLastProcesses(user, contractnumber, processtype);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result[0].ProcessStatus);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
        }
    }
}
