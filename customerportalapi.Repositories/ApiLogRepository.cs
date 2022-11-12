using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly IMongoCollectionWrapper<ApiLog> _apiLog;

        public ApiLogRepository(IConfiguration config, IMongoCollectionWrapper<ApiLog> apiLog)
        {
            _apiLog = apiLog;
        }

        public ApiLog Update(ApiLog apiLog)
        {
            var filter = Builders<ApiLog>.Filter.Eq(s => s.Username, apiLog.Username);
            var result = _apiLog.ReplaceOne(filter, apiLog);

            return apiLog;
        }

        public ApiLog UpdateById(ApiLog apiLog)
        {
            var filter = Builders<ApiLog>.Filter.Eq(s => s.Id, apiLog.Id);
            var result = _apiLog.ReplaceOne(filter, apiLog);

            return apiLog;
        }

        public Task<bool> Create(ApiLog apiLog)
        {
            _apiLog.InsertOne(apiLog);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(ApiLog apiLog)
        {
            var filter = Builders<ApiLog>.Filter.Eq("username", apiLog.Username);
            _apiLog.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }
    }
}
