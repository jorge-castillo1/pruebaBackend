using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using customerportalapi.Services.Interfaces;
using customerportalapi.Repositories.interfaces;
using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using customerportalapi.Services.Exceptions;
using System.Threading.Tasks;

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

        public List<Process> GetLastProcesses(string user, string smContractCode, int? processtype)
        {
            ProcessSearchFilter filter = new ProcessSearchFilter()
            {
                UserName = user,
                SmContractCode = smContractCode,
                ProcessType = processtype
            };
            List<Process> processes = _processRepository.Find(filter);

            List<Process> ordered = processes.OrderBy(item => item.SmContractCode).ThenBy(item => item.ProcessType).ThenByDescending(item => item.ModifiedDate).ToList();
            List<Process> last = new List<Process>();

            if (processes.Count == 0) return last;
            last.Add(ordered[0]);
            string lastSmContractCode = ordered[0].SmContractCode;
            int lastProcesstype = ordered[0].ProcessType;
            foreach(var process in ordered)
            {
                if (process.SmContractCode != lastSmContractCode || process.ProcessType != lastProcesstype)
                {
                    last.Add(process);
                    lastSmContractCode = process.SmContractCode;
                    lastProcesstype = process.ProcessType;
                }
            }
            return last;
        }

        public bool CancelProcess(string smContractCode, int processtype)
        {
            List<Process> processes = GetLastProcesses(null, smContractCode, processtype);
            if (processes.Count == 0) throw new ServiceException("Process not found", HttpStatusCode.NotFound);
            Process process = processes[0];
            process.ProcessStatus = (int)ProcessStatuses.Canceled;
            _processRepository.Update(process);

            if (process.ProcessType == (int)ProcessTypes.PaymentMethodChangeBank)
            {
                foreach(ProcessDocument processdocument in process.Documents)
                    _signatureRepository.CancelSignature(processdocument.DocumentId);
            }
            return true;
        }

        public Process UpdateSignatureProcess(SignatureStatus value)
        {
            if (value.Status != "document_completed" && value.Status != "document_canceled")
                throw new ServiceException("Document status not valid", HttpStatusCode.BadRequest);

            ProcessSearchFilter filter = new ProcessSearchFilter
            {
                UserName = value.User,
                DocumentId = value.DocumentId
            };

            List<Process> processes = _processRepository.Find(filter);
            //if (processes.Count == 0) throw new ServiceException("Process not found.", HttpStatusCode.NotFound);
            if (processes.Count == 0) return null;
            if (processes.Count > 1) throw new ServiceException("More than one process was found", HttpStatusCode.BadRequest);

            Process process = processes[0];
            bool processCompleted = true;
            bool processCanceled = false;
            foreach (ProcessDocument processDocument in process.Documents)
            {
                if (processDocument.DocumentId == value.DocumentId)
                    processDocument.DocumentStatus = value.Status;

                processCompleted = processCompleted && processDocument.DocumentStatus == "document_completed";
                processCanceled = processCanceled || processDocument.DocumentStatus == "document_canceled";
            }

            //When all documents are completed, process will be completed.
            //When one document are cancelled, all process will be cancelled
            if (processCompleted) process.ProcessStatus = (int)ProcessStatuses.Accepted;
            else if (processCanceled) process.ProcessStatus = (int)ProcessStatuses.Canceled;
              
            _processRepository.Update(process);

            return process;
        }
    }
}
