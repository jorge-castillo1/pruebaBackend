using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Services.interfaces
{
    public interface IPaymentService
    {
        Task<bool> ChangePaymentMethod(PaymentMethod paymentMethod);
        Task UpdatePaymentProcess(SignatureStatus value);
    }
}
