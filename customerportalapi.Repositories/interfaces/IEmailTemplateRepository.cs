using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IEmailTemplateRepository
    {
        EmailTemplate getTemplate(int templatecode, string language);
    }
}
