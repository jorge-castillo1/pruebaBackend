using System;
using customerportalapi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IContractSMRepository
    {
        Task<SMContract> GetAccessCodeAsync(string contractId);
        Task<List<Invoice>> GetInvoicesAsync(string contractId);
        Task<bool> MakePayment(MakePayment makePayment);
    }
}
