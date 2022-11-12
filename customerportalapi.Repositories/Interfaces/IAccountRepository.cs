using customerportalapi.Entities;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IUserAccountRepository
    {
        UserAccount GetAccount(string username);
        UserAccount Update(UserAccount account);
        UserAccount UpdateById(UserAccount account);
        Task<bool> Create(UserAccount account);
        Task<bool> Delete(UserAccount account);
    }
}
