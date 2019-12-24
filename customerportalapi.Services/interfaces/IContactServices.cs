using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IContactServices
    {
        Task<Contact> GetContactAsync(string dni);
    }
}
