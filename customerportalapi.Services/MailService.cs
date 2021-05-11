using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Services.Interfaces;
using System.Threading.Tasks;

namespace customerportalapi.Services
{
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;


        public MailService(IMailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }

        public async Task<bool> Send(Email email)
        {
            var res = await _mailRepository.Send(email);
            return res;
        }
    }
}
