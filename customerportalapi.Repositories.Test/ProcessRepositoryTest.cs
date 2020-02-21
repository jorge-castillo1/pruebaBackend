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
    public class ProcessRepositoryTest
    {
        private IConfigurationRoot _configurations;
        private Mock<IMongoCollectionWrapper<Process>> _processes;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _processes = new Mock<IMongoCollectionWrapper<Process>>();
            _processes.Setup(x => x.Find(It.IsAny<FilterDefinition<Process>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<FindOptions>())).Returns(
                new List<Process>
                {
                    new Process
                    {
                        Id = "fake Id",
                        Username = "fake username",
                        ProcessType = 0, //Cambio metodo de pago
                        ProcessStatus = 0, //Pendiente
                        DocumentId = Guid.NewGuid().ToString(), //Optional
                        ContractNumber = "Fake Contract Number", //Optional
                        CreationDate = System.DateTime.Now,
                        ModifiedDate = System.DateTime.Now
                    },
                    new Process
                    {
                        Id = "fake Id2",
                        Username = "fake username",
                        ProcessType = 1, //Cambio metodo de pago
                        ProcessStatus = 0, //Pendiente
                        CreationDate = System.DateTime.Now,
                        ModifiedDate = System.DateTime.Now
                    }
                });
            _processes.Setup(x => x.InsertOne(It.IsAny<Process>())).Verifiable();
            _processes.Setup(x => x.ReplaceOne(It.IsAny<FilterDefinition<Process>>(), It.IsAny<Process>())).Returns(new Mock<ReplaceOneResult>().Object).Verifiable();
        }

        [TestMethod]
        public async Task AlCrearUnProceso_NoSeProducenErrores()
        {
            //Arrange
            Process process = new Process();
            process.Username = "fake user";
            process.ProcessType = 0;
            process.ProcessStatus = 0;
            process.DocumentId = "fake document id";
            process.ContractNumber = "fake contract number";
            
            //Act
            ProcessRepository repository = new ProcessRepository(_configurations, _processes.Object);
            bool result = await repository.Create(process);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AlActualizarUnProceso_NoSeProducenErrores()
        {
            //Arrange
            Process process = new Process();
            process.Id = "fake id";
            process.Username = "fake user";
            process.ProcessType = 0;
            process.ProcessStatus = 0;
            process.DocumentId = "fake document id";
            process.ContractNumber = "fake contract number";
            
            //Act
            ProcessRepository repository = new ProcessRepository(_configurations, _processes.Object);
            Process updatedEntity = repository.Update(process);

            //Assert
            Assert.IsTrue(updatedEntity.ModifiedDate != DateTime.MinValue);
        }

        [TestMethod]
        public void AlRecuperarUnProceso_NoSeProducenErrores()
        {
            //Arrange
            ProcessSearchFilter filter = new ProcessSearchFilter();
            filter.UserName = "fake username";
            filter.ProcessType = 0;

            //Act
            ProcessRepository repository = new ProcessRepository(_configurations, _processes.Object);
            List<Process> results = repository.Find(filter);

            Assert.AreEqual(2, results.Count);
        }

        
    }
}
