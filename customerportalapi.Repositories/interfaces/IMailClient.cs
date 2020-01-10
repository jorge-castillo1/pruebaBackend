using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IMailClient
    {
        Task SendAsync(MimeMessage message);
        void Disconnect(bool disconnect);
        void Dispose();
    }
}
