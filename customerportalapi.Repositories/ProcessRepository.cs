using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class ProcessRepository : IProcessRepository
    {
        private readonly IMongoCollectionWrapper<Process> _processes;

        public ProcessRepository(IConfiguration config, IMongoCollectionWrapper<Process> processes)
        {
            _processes = processes;
        }

        public Task<bool> Create(Process process)
        {
            //create process
            process.CreationDate = System.DateTime.Now;
            process.ModifiedDate = System.DateTime.Now;

            _processes.InsertOne(process);
            return Task.FromResult(true);
        }

        public Process Update(Process process)
        {
            //update Process
            process.ModifiedDate = System.DateTime.Now;

            var filter = Builders<Process>.Filter.Eq(s => s.Id, process.Id);
            var result = _processes.ReplaceOne(filter, process);

            return process;
        }

        public List<Process> Find(ProcessSearchFilter filter)
        {
            FilterDefinition<Process> filters = Builders<Process>.Filter.Empty;

            if (!string.IsNullOrEmpty(filter.UserName))
                filters = filters & Builders<Process>.Filter.Eq(x => x.Username, filter.UserName);

            if (filter.ProcessType.HasValue)
                filters = filters & Builders<Process>.Filter.Eq(x => x.ProcessType, filter.ProcessType.Value);

            if (!string.IsNullOrEmpty(filter.SmContractCode))
                filters = filters & Builders<Process>.Filter.Eq(x => x.SmContractCode, filter.SmContractCode);

            if (!string.IsNullOrEmpty(filter.DocumentId))
                filters = filters & Builders<Process>.Filter.ElemMatch(x => x.Documents, a => a.DocumentId == filter.DocumentId);

            if (filter.ProcessStatus.HasValue)
                filters = filters & Builders<Process>.Filter.Eq(x => x.ProcessStatus, filter.ProcessStatus.Value);

            //Without pagination by the moment
            return _processes.Find(filters, 1, 0);
        }
    }
}
