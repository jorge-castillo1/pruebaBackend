using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IBearBoxRepository
    {
        Task<BearBoxStorageUserResponse> GetUser(string smCustomerId);
        Task<BearBoxPinResponse> GetPIN(string userId);
        Task<BearBoxPinResponse> UpdatePINAsync(BearBoxPinRequest user);
    }
}
