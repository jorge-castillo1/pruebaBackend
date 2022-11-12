using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities.Enums;

namespace customerportalapi.Services.Interfaces
{
    public interface IWebTemplateServices
    {
        Task<List<WebTemplate>> GetTemplates();
        Task<WebTemplate> GetTemplate(WebTemplateTypes templateCode, string language);
    }
}
