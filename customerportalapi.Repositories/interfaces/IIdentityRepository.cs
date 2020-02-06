using System;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IIdentityRepository
    {
        Task<Token> Authorize(Login credentials);
        Task<TokenStatus> Validate(string token);

        Task<UserIdentity> AddUser(UserIdentity userIdentity);
        Task<UserIdentity> UpdateUser(UserIdentity userIdentity);
        Task<UserIdentity> GetUser(string userId);
    }
}
