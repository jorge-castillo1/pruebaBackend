using customerportalapi.Entities;
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
        private Mock<IUserRepository> _userRepository;
        private Mock<IProcessRepository> _processRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = UserRepositoryMock.ValidUserRepository();
            _processRepository = ProcessRepositoryMock.ProcessRepository();
        }

        [TestMethod]
        public void SiSeEncuentranProcesos_DevuelveElMasReciente()
        {
            //Arrange
            string user = "fake user";
            string contractnumber = "fake contract";
            int processtype = 1;

            _processRepository = ProcessRepositoryMock.MoreThanOneResultProcessRepository();
            ProcessService service = new ProcessService(_processRepository.Object);
            var result = service.GetLastProcesses(user, contractnumber, processtype);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result[0].ProcessStatus);
            _processRepository.Verify(x => x.Find(It.IsAny<ProcessSearchFilter>()));
        }
    }
}
