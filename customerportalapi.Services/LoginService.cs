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

        public LoginService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public async Task<Token> GetToken(Login credentials){

            return await _identityRepository.Authorize(credentials);
        }

    }
}