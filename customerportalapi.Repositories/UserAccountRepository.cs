using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;

namespace customerportalapi.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IMongoCollectionWrapper<UserAccount> _userAccount;

        public UserAccountRepository(IConfiguration config, IMongoCollectionWrapper<UserAccount> userAccount)
        {
            _userAccount = userAccount;
        }

        public UserAccount GetAccount(string username)
        {
            UserAccount userAccount = new UserAccount();

            var usersInfo = _userAccount.FindOne(t => t.Username == username);
            foreach (var u in usersInfo)
            {
                userAccount = u;
            }
            return userAccount;
        }

        public UserAccount Update(UserAccount userAccount)
        {
            //update User
            var filter = Builders<UserAccount>.Filter.Eq(s => s.Username, userAccount.Username);
            var result = _userAccount.ReplaceOne(filter, userAccount);

            return userAccount;
        }

        public UserAccount UpdateById(UserAccount userAccount)
        {
            var filter = Builders<UserAccount>.Filter.Eq(s => s.Id, userAccount.Id);
            var result = _userAccount.ReplaceOne(filter, userAccount);

            return userAccount;
        }

        public Task<bool> Create(UserAccount userAccount)
        {
            //update User
            _userAccount.InsertOne(userAccount);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(UserAccount userAccount)
        {
            //update User
            var filter = Builders<UserAccount>.Filter.Eq("username", userAccount.Username);
            _userAccount.DeleteOneAsync(filter);

            return Task.FromResult(true);
        }

    }
}
