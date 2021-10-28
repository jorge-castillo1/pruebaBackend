using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IConfiguration _config;

        public MailService(IMailRepository mailRepository,
            IConfiguration config)
        {
            _mailRepository = mailRepository;
            _config = config;
        }

        public Task<bool> SendEmail(Entities.Email email)
        {
            return _mailRepository.Send(email);
        }
    }
}