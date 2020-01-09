using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class MailRepositoryTest
    {
        IConfiguration _configurations;
        Mock<IMailClient> _mailclient;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _mailclient = new Mock<IMailClient>();
            _mailclient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
        }

        [TestMethod]
        public void AlEnviarCorreo_NoSeProducenErrores()
        {
            //Arrange
            string confirmUrl = "https://localhost:44332/api/users/acceptinvitation";
            string confirmText = "Confirm email";

            Email mailmessage = new Email();
            mailmessage.To = new List<string>()
            {
                "jordi.gimenez@quantion.com"
            };

            mailmessage.Cc = new List<string>()
            {
                "jordi.gimenez@quantion.com"
            };
            mailmessage.Subject = "Welcome Bluespace private customer portal.";
            mailmessage.Body = String.Format("Bluespace invite you to access private customer portal. Click the link below to confirm your email address and gain portal access <a href='{0}'>{1}</a>", confirmUrl, confirmText);

            //Act
            MailRepository _mailrepository = new MailRepository(_configurations, _mailclient.Object);
            bool result = _mailrepository.Send(mailmessage).Result;

            //Assert
            Assert.IsTrue(result);
            //_smtpclient.Verify(x => x.Send(It.IsAny<MailMessage>()));
        }
    }
}
