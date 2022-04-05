using customerportalapi.Repositories.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Utils
{
    public class MailClientWrapper : IMailClient
    {
        SmtpClient _client;

        public MailClientWrapper(SmtpClient client)
        {
            _client = client;
        }

        public async Task SendAsync(MimeMessage message)
        {
            await _client.SendAsync(message);
        }

        public void Disconnect(bool disconnect)
        {
            _client.Disconnect(disconnect);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
