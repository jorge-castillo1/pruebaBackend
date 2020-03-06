using System;
using System.Collections.Generic;
using System.Text;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface IProcessService
    {
        List<Process> GetLastProcesses(string user, string contractnumber, int? processtype);
        bool CancelProcess(string contractnumber, int processtype);
    }
}
