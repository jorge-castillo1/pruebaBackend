using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;

namespace customerportalapi.Services.interfaces
{
    public interface IWebTemplateServices
    {
        Task<List<WebTemplate>> GetTemplates();
        Task<WebTemplate> GetTemplate(WebTemplateTypes templateCode, string language);
    }
}
