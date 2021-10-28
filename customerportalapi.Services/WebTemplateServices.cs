using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities.enums;
using customerportalapi.Services.Interfaces;

namespace customerportalapi.Services
{
    public class WebTemplateServices : IWebTemplateServices
    {
        private readonly IWebTemplateRepository _templateRepository;

        public WebTemplateServices(IWebTemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<List<WebTemplate>> GetTemplates()
        {
            var entitylist = _templateRepository.GetTemplates();

            return await Task.FromResult(entitylist);
        }

        public async Task<WebTemplate> GetTemplate(WebTemplateTypes templateCode, string language)
        {
            var entity = _templateRepository.GetTemplate((int)templateCode, language);

            return await Task.FromResult(entity);
        }
    }
}
