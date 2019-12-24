using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IUserRepository : IDisposable
    {
        User getCurrentUser(string account);
        User update(User user);
        Task<bool> create(User user);
        Task<bool> delete(User user);
    }
}
