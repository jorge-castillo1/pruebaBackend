﻿using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace customerportalapi.Repositories
{
    public class MailRepository : IMailRepository, IDisposable
    {
        IConfiguration _configuration;
        private IMailClient _mailClient;
        private readonly ILogger<MailRepository> _logger;

        public MailRepository(IConfiguration configuration, IMailClient mailClient, ILogger<MailRepository> logger)
        {
            _configuration = configuration;
            _mailClient = mailClient;
            _logger = logger;
        }

        public async Task<bool> SendNotDisconnect(Email messageData)
        {
            return await Send(messageData, false);
        }

        public async Task<bool> Send(Email messageData, bool disconnect = true)
        {
            var msCC = AddEmailCCandCCOfromConfig(messageData);
            var msSplit = SplitEmail(msCC);
            var msData = DeleteWrongMailAdressCCandCCO(msSplit);

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

            LogTrySendMessage(messageData);

            await _mailClient.SendAsync(message);
            if (disconnect) _mailClient.Disconnect(true);

            if (messageData.EmailFlow != null) _logger.LogInformation($"MailRepository.Send().  {messageData.EmailFlow} SENDSUCCESSFULL ");
            return true;
        }

        private void LogTrySendMessage(Email messageData)
        {
            if (messageData != null)
            {
                string to = "";
                foreach (var k in messageData.To)
                {
                    to += k + ";";
                }
                string cc = "";
                foreach (var k in messageData.Cc)
                {
                    cc += k + ";";
                }
                string cco = "";
                foreach (var k in messageData.Cco)
                {
                    cco += k + ";";
                }

                _logger.LogInformation($"MailRepository.LogTrySendMessage(). {messageData.EmailFlow} TRYSENDMESSAGE  To :{to} || CC : {cc} || CCO : {cco} ");
            }
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

        private Email DeleteWrongMailAdressCCandCCO(Email emailObject)
        {
            emailObject.Cc.RemoveAll(direction => !IsValid(direction));
            emailObject.Cco.RemoveAll(direction => !IsValid(direction));
            return emailObject;
        }


        public bool IsValid(string emailaddress)
        {
            try
            {
                MailboxAddress v = new MailboxAddress(emailaddress);
                Match match = Regex.Match(emailaddress, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                if (match.Success)
                {
                    return true;
                }
                else
                {
                    _logger.LogInformation($"MailRepository.IsValid().  WRONG MAIL ADDRESS : {emailaddress} ");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"MailRepository.IsValid().  WRONG MAIL ADDRESS : {emailaddress} ");
                return false;
            }
        }

        public void Dispose()
        {
            _mailClient.Dispose();
        }
    }
}