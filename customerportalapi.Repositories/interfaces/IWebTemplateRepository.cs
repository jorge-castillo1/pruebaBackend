using System.Collections.Generic;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.interfaces
{
    public interface IWebTemplateRepository
    {
        List<WebTemplate> GetTemplates();
        WebTemplate GetTemplate(int templatecode, string language);
    }
}
