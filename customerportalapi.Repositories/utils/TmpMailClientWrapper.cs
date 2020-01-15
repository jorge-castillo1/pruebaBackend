using customerportalapi.Repositories.interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.utils
{
    public class TmpMailClientWrapper : IMailClient
    {
        SmtpClient _client;

        public TmpMailClientWrapper(SmtpClient client)
        {
            _client = client;
        }

        public void Disconnect(bool disconnect)
        {
            
        }

        public void Dispose()
        {
            
        }

        public async Task SendAsync(MimeMessage message)
        {
            await Task.CompletedTask;
        }
    }
}
