using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Repositories
{
    public abstract class DatabaseRepository
    {
        //private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        protected IMongoDatabase Database { get; }

        public DatabaseRepository(IConfiguration config) 
        {
            _config = config;
            
            var client = new MongoClient(_config.GetConnectionString("customerportaldb"));
            this.Database = client.GetDatabase("customerportal");
        }
    }
}
