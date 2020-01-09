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
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration["MailFrom"]);
            message.To.Add(string.Join(",", messageData.To));
            message.Subject = string.IsNullOrEmpty(messageData.Subject) ? " " : messageData.Subject;
            message.Body = messageData.Body;
            message.IsBodyHtml = true;
            
            await _mailClient.SendMailAsync(message);

            return true;
        }

        public void Dispose()
        {
            _mailClient.Dispose();
        }
    }
}
