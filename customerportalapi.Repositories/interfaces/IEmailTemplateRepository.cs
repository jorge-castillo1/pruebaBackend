using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Repositories.interfaces
{
    public interface IEmailTemplateRepository
    {
        EmailTemplate getTemplate(int templatecode, string language);
    }
}
