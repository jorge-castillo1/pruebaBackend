using System;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.Interfaces
{
    public interface IContractSMRepository
    {
        Task<SMContract> GetAccessCodeAsync(string contractId);
        Task<List<Invoice>> GetInvoicesAsync(string contractId);
        Task<List<Invoice>> GetInvoicesByCustomerIdAsync(string cutomerId);
        Task<bool> MakePayment(MakePayment makePayment);
        Task<SubContract> GetSubContractAsync(string contractId, string unitId);
        Task<bool> UpdateAccessCodeAsync(UpdateAccessCode updateAccessCode);
        Task<ApsData> UpdateAps(ApsRequest request);
        Task<List<ApsData>> GetApssByField(string field, string code);
    }
}
