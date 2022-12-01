using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IBearBoxRepository
    {
        Task<object> GetUser(string smCustomerId);
        Task<object> GetPIN(string userId);
        Task<object> UpdatePINAsync(object user);
    }
}
