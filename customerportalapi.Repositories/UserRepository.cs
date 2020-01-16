using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;

namespace customerportalapi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollectionWrapper<User> _users;

        public UserRepository(IConfiguration config, IMongoCollectionWrapper<User> users)
        {
            _users = users;
        }

        public User GetCurrentUser(string dni)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.dni == dni);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }

        public User Update(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq(s => s.dni, user.dni);
            var result = _users.ReplaceOne(filter, user);

            return user;
        }

        public Task<bool> Create(User user)
        {
            //update User
            _users.InsertOne(user);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq("dni", user.dni);
            _users.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

        public User GetUserByInvitationToken(string invitationToken)
        {
            User user = new User();

            var usersInfo = _users.FindOne(t => t.invitationtoken == invitationToken);
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }
    }
}
