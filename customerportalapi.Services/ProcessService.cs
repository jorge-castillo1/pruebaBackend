using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using customerportalapi.Services.Interfaces;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Services.Exceptions;

namespace customerportalapi.Services
{
    public class ProcessService : IProcessService
    {
        private readonly IProcessRepository _processRepository;
        private readonly ISignatureRepository _signatureRepository;
        public ProcessService(IProcessRepository processRepository, ISignatureRepository signatureRepository)
        {
            _processRepository = processRepository;
            _signatureRepository = signatureRepository;
        }

        public List<Process> GetLastProcesses(string user, string contractnumber, int? processtype)
        {
            ProcessSearchFilter filter = new ProcessSearchFilter()
            {
                UserName = user,
                ContractNumber = contractnumber,
                ProcessType = processtype
            };
            List<Process> processes = _processRepository.Find(filter);

            List<Process> ordered = processes.OrderBy(item => item.ContractNumber).ThenBy(item => item.ProcessType).ThenByDescending(item => item.ModifiedDate).ToList();
            List<Process> last = new List<Process>();

            if (processes.Count == 0) return last;
            last.Add(ordered[0]);
            string lastContractnumber = ordered[0].ContractNumber;
            int lastProcesstype = ordered[0].ProcessType;
            foreach(var process in ordered)
            {
                if (process.ContractNumber != lastContractnumber || process.ProcessType != lastProcesstype)
                {
                    last.Add(process);
                    lastContractnumber = process.ContractNumber;
                    lastProcesstype = process.ProcessType;
                }
            }
            return last;
        }

        public bool CancelProcess(string contractnumber, int processtype)
        {
            List<Process> processes = GetLastProcesses(null, contractnumber, processtype);
            if (processes.Count == 0) throw new ServiceException("Process not found", HttpStatusCode.NotFound);
            Process process = processes[0];
            var documentId = process.DocumentId;
            process.ProcessStatus = (int)ProcessStatuses.Canceled;
            _processRepository.Update(process);
            _signatureRepository.CancelSignature(documentId);    
            return true;
        }
    }
}
