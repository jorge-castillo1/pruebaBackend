using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories
{
    public class MailRepository : IMailRepository, IDisposable
    {
        IConfiguration _configuration;
        private IMailClient _mailClient;

        public MailRepository(IConfiguration configuration, IMailClient mailClient) 
        {
            _configuration = configuration;
            _mailClient = mailClient;
        }

        public async Task<bool> Send(Email messageData)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["MailFrom"]));

            foreach (string address in messageData.To)
                message.To.Add(new MailboxAddress(address));

            if (messageData.Cc.Count == 1 && messageData.Cc[0] == "")
            {
                foreach (string address in messageData.To)
                    message.Cc.Add(new MailboxAddress(address));
            }
            else
            {
                foreach (string address in messageData.Cc)
                    message.Cc.Add(new MailboxAddress(address));
            }
            message.Subject = string.IsNullOrEmpty(messageData.Subject) ? " " : messageData.Subject;
            if (!string.IsNullOrEmpty(messageData.Body))
            {
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = messageData.Body
                };
            }
            else
            {
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = " "
                };
            }

            await _mailClient.SendAsync(message);
            _mailClient.Disconnect(true);

            return true;
        }

        public void Dispose()
        {
            _mailClient.Dispose();
        }
    }
}
