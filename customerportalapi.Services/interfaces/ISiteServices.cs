using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface ISiteServices
    {
        Task<List<Site>> GetContractsAsync(string dni);
    }
}
