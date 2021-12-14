using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;

namespace customerportalapi.Repositories
{
    public class NewUserRepository : INewUserRepository
    {
        private readonly IMongoCollectionWrapper<NewUser> _newUsers;

        public NewUserRepository(IConfiguration config, IMongoCollectionWrapper<NewUser> users)
        {
            _newUsers = users;
        }

        public Task<bool> SaveNewUser(NewUser user)
        {
            _newUsers.InsertOne(user);

            return Task.FromResult(true); ;
        }
    }
}
