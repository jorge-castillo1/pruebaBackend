using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IMailRepository
    {
        Task<bool> Send(Email messageData, bool disconnect = true);

        Task<bool> SendNotDisconnect(Email messageData);
    }
}
