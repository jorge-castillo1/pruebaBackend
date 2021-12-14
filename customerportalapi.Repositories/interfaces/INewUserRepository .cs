using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.interfaces
{
    public interface INewUserRepository
    {
        Task<bool> SaveNewUser(NewUser user);
    }
}
