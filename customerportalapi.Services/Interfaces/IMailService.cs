using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> Send(Email email);
    }
}
