using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Repositories.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Test
{
    [TestClass]
    public class MailRepositoryTest
    {
        IConfiguration _configurations;
        Mock<IMailClient> _mailclient;
        Mock<ILogger<MailRepository>> _logger;


        [TestInitialize]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            _configurations = builder.Build();

            _mailclient = new Mock<IMailClient>();
            _mailclient.Setup(x => x.SendAsync(It.IsAny<MimeMessage>())).Returns(Task.CompletedTask);
            _mailclient.Setup(x => x.Disconnect(It.IsAny<bool>())).Verifiable();
            _mailclient.Setup(x => x.Dispose()).Verifiable();
            _logger = new Mock<ILogger<MailRepository>>();
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
            mailmessage.EmailFlow = EmailFlowType.SendWelcome.ToString();

           //Act
            MailRepository _mailrepository = new MailRepository(_configurations, _mailclient.Object,_logger.Object);
            bool result = _mailrepository.Send(mailmessage).Result;

            //Assert
            Assert.IsTrue(result);
            _mailclient.Verify(x => x.SendAsync(It.IsAny<MimeMessage>()));
            _mailclient.Verify(x => x.Disconnect(It.IsAny<bool>()));
        }


        [TestMethod]
        public void AlEnviarCorreoConMultiplesEmails_NoSeProducenErrores()
        {
            //Arrange
            Email mailmessage = new Email();
            mailmessage.To = new List<string>()
            {
                "daniel.vazquez@quantion.com,jorge.castillo@quantion.com"
            };

            mailmessage.Cc = new List<string>()
            {
                "daniel.vazquez@quantion.com;jorge.castillo@quantion.com"
            };

            mailmessage.Cco = new List<string>()
            {
                "daniel.vazquez@quantion.com;jorge.castillo@quantion.com"
            };

            mailmessage.Subject = "Prueba envio correo multiples recipients To,CC,CCO";
            mailmessage.Body = String.Format("Prueba envio correo multiples recipients To,CC,CCO");
            mailmessage.EmailFlow = EmailFlowType.Contact.ToString();

            //Act
            MailRepository _mailrepository = new MailRepository(_configurations, _mailclient.Object, _logger.Object);
            bool result = _mailrepository.Send(mailmessage).Result;

            //Assert
            Assert.IsTrue(result);
            _mailclient.Verify(x => x.SendAsync(It.IsAny<MimeMessage>()));
            _mailclient.Verify(x => x.Disconnect(It.IsAny<bool>()));
        }
    }
}
