using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IApiLogService
    {
        Task<bool> AddLog(ApiLog apiLog);
    }
}
