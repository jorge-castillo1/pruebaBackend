using customerportalapi.Entities;
using customerportalapi.Repositories.interfaces;
using System.Collections.Generic;

namespace customerportalapi.Repositories
{
    public class WebTemplateRepository : IWebTemplateRepository
    {
        private readonly IMongoCollectionWrapper<WebTemplate> _templates;

        public WebTemplateRepository(IMongoCollectionWrapper<WebTemplate> templates)
        {
            _templates = templates;
        }

        public List<WebTemplate> GetTemplates()
        {
            return _templates.FindAll(t => true);
        }

        public WebTemplate GetTemplate(int templateCode, string language)
        {
            WebTemplate template = new WebTemplate();

            var templatesInfo = _templates.FindOne(t => t.Code == templateCode && t.Language == language);
            foreach (var t in templatesInfo)
            {
                template = t;
            }
            return template;
        }
    }
}
