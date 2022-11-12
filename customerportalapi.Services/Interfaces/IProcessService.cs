using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Services.Interfaces
{
    public interface IProcessService
    {
        List<Process> GetLastProcesses(string user, string smContractCode, int? processtype);
        bool CancelProcess(string contractnumber, int processtype);
        Process UpdateDocumentStatusProcess(SignatureStatus value);
        int CancelAllProcessesByUsernameAndProcesstype(string username, int processtype);
    }
}
