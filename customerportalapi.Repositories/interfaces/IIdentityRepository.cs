using System;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IIdentityRepository
    {
        Task<Token> Authorize(Login credentials);
        Task<Token> RefreshToken(string token);
        Task<bool> Logout(string token);
    }
}
