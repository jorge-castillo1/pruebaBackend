using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using customerportalapi.Services.Exceptions;

namespace customerportalapi.Services
{
    public class LoginService : ILoginService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IUserRepository _userRepository;

        public LoginService(IIdentityRepository identityRepository, IUserRepository userRepository)
        {
            _identityRepository = identityRepository;
            _userRepository = userRepository;
        }

        public async Task<Token> GetToken(Login credentials){

            return await _identityRepository.Authorize(credentials);
        }

        public async Task<UserIdentity> ChangePassword(ResetPassword credentials)
        {
            //1. Get User From backend
            User currentUser = _userRepository.GetCurrentUser(credentials.Username);

            //2. Validate Old Password is valid
            //¿pedir token?

            //3. Update user
            UserIdentity user = await _identityRepository.GetUser(currentUser.ExternalId);
            if (user != null)
            {
                user.Password = credentials.NewPassword;
                user = await _identityRepository.UpdateUser(user);
            }

            return user;
        }
    }
}