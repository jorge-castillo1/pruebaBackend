using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IUserServices
    {
        Task<Profile> GetProfileAsync(string dni);

        Task<Profile> UpdateProfileAsync(Profile profile);
    }
}
