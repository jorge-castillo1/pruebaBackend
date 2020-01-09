using customerportalapi.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.utils
{
    public class MailClientWrapper : IMailClient
    {
        SmtpClient _client;

        public MailClientWrapper(SmtpClient client)
        {
            _client = client;
        }

        public async Task SendMailAsync(MailMessage message)
        {
            await _client.SendMailAsync(message);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
