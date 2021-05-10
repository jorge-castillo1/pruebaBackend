using customerportalapi.Entities;
using customerportalapi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class EmailServices : IEmailService
    {
        public Task<bool> Send(Email email)
        {
            throw new NotImplementedException();
        }
    }
}
