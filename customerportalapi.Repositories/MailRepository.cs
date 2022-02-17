using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var msCC = AddEmailCCandCCOfromConfig(messageData);
            var msData = SplitEmail(msCC);

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["MailFrom"]));

            foreach (string address in msData.To)
                message.To.Add(new MailboxAddress(address));


            foreach (string addCCO in msData.Cco)
            {
                message.Bcc.Add(new MailboxAddress(addCCO));
            }

            if (msData.Cc.Count == 1 && msData.Cc[0] == "")
            {
                foreach (string address in msData.To)
                    message.Cc.Add(new MailboxAddress(address));
            }
            else
            {
                foreach (string address in msData.Cc)
                    message.Cc.Add(new MailboxAddress(address));
            }
            message.Subject = string.IsNullOrEmpty(msData.Subject) ? " " : msData.Subject;
            if (!string.IsNullOrEmpty(msData.Body))
            {
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = msData.Body
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

        public Email AddEmailCCandCCOfromConfig(Email messageData)
        {
            if (!string.IsNullOrEmpty(messageData.EmailFlow))
            {

                var ccoMail = $"Mail{messageData.EmailFlow}CCO";
                if (!string.IsNullOrEmpty(_configuration[ccoMail]))
                {
                    messageData.Cco.Add(_configuration[ccoMail]);
                }

                var ccMail = $"Mail{messageData.EmailFlow}CC";
                if (!string.IsNullOrEmpty(_configuration[ccMail]))
                {
                    messageData.Cc.Add(_configuration[ccMail]);
                }
            }

            return messageData;
        }

        public Email SplitEmail(Email messageData)
        {
            List<string> mailto = new List<string>();
            foreach (var mail in messageData.To)
            {
                List<string> m = mail.Split(';', ',').ToList();
                mailto.AddRange(m);
            }
            messageData.To.Clear();
            messageData.To.AddRange(mailto);

            List<string> mailcc = new List<string>();
            foreach (var mail in messageData.Cc)
            {
                List<string> m = mail.Split(';', ',').ToList();
                mailcc.AddRange(m);
            }
            messageData.Cc.Clear();
            messageData.Cc.AddRange(mailcc);


            List<string> mailcco = new List<string>();
            foreach (var mail in messageData.Cco)
            {
                List<string> m = mail.Split(';', ',').ToList();
                mailcco.AddRange(m);
            }
            messageData.Cco.Clear();
            messageData.Cco.AddRange(mailcco);

            return messageData;
        }

        public void Dispose()
        {
            _mailClient.Dispose();
        }
    }
}
