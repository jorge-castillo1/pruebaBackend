using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IApiLogRepository
    {
        ApiLog Update(ApiLog apiLog);
        ApiLog UpdateById(ApiLog apiLog);
        Task<bool> Create(ApiLog apiLog);
        Task<bool> Delete(ApiLog apiLog);
    }
}
