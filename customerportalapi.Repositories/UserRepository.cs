using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;

namespace customerportalapi.Repositories
{
    public class UserRepository : DatabaseRepository, IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IConfiguration config) : base(config)
        {
            _users = Database.GetCollection<User>("users");
        }
        public void Dispose()
        {

        }

        public User getCurrentUser(string dni)
        {
            User user = new User();

            var usersInfo = _users.Find(t => t.dni == dni).Limit(1).ToList();
            foreach (var u in usersInfo)
            {
                user = u;
            }
            return user;
        }
        public User update(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq(s => s.dni, user.dni);
            var result = _users.ReplaceOneAsync(filter, user);

            return user;
        }
        public Task<bool> create(User user)
        {
            //update User
            _users.InsertOne(user);

            return Task.FromResult<bool>(true);
        }

        public Task<bool> delete(User user)
        {
            //update User
            var filter = Builders<User>.Filter.Eq("dni", user.dni);
            _users.DeleteOneAsync(filter);

            return Task.FromResult<bool>(true);
        }
    }
}
