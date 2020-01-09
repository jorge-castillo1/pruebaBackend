using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IMailClient
    {
        Task SendMailAsync(MailMessage message);
        void Dispose();
    }
}
