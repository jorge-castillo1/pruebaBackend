using customerportalapi.Entities;
using customerportalapi.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly IMongoCollectionWrapper<EmailTemplate> _templates;

        public EmailTemplateRepository(IConfiguration config, IMongoCollectionWrapper<EmailTemplate> templates)
        {
            _templates = templates;
        }

        public EmailTemplate getTemplate(int templatecode, string language)
        {
            EmailTemplate template = new EmailTemplate();

            var templatesInfo = _templates.FindOne(t => t.code == templatecode && t.language == language);
            foreach (var t in templatesInfo)
            {
                template = t;
            }
            return template;
        }
    }
}
