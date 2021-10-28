using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendEmail(Entities.Email email);
    }
}