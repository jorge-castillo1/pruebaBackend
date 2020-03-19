using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface IProcessService
    {
        List<Process> GetLastProcesses(string user, string contractnumber, int? processtype);
        bool CancelProcess(string contractnumber, int processtype);
        Process UpdateSignatureProcess(SignatureStatus value);
    }
}
