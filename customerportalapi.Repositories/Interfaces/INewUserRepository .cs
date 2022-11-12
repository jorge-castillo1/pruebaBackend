using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.Interfaces
{
    public interface INewUserRepository
    {
        Task<bool> SaveNewUser(NewUser user);
    }
}
