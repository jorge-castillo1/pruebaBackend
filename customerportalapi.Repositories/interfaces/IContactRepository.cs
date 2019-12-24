using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IContactRepository
    {
       Task<Contact> GetContactAsync(string dni);
    }
}
