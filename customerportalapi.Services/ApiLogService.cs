using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using customerportalapi.Services.Interfaces;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class ApiLogService : IApiLogService
    {
        private readonly IApiLogRepository _apiLogRepository;

        public ApiLogService(IApiLogRepository apiLogRepository)
        {
            _apiLogRepository = apiLogRepository;
        }

        public async Task<bool> AddLog(ApiLog apiLog)
        {
            return _apiLogRepository.Create(apiLog).Result;
        }
    }
}