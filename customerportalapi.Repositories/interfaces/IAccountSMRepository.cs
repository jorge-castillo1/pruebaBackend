using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.interfaces
{
    public interface IAccountSMRepository
    {
        Task<bool> AddBankAccountAsync(SMBankAccount account);
    }
}
