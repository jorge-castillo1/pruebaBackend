using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IProcessRepository
    {
        Task<bool> Create(Process process);
        Process Update(Process process);
        List<Process> Find(ProcessSearchFilter filter);
    }
}
