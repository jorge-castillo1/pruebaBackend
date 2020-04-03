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
    public class AuthService : IAuthService
    {
        private readonly IIdentityRepository _identityRepository;

        public AuthService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public async Task<Token> RefreshToken(string token)
        {
            return await _identityRepository.RefreshToken(token);
        }

        public async Task<bool> Logout(string token)
        {
            return await _identityRepository.Logout(token);
        }

    }
}