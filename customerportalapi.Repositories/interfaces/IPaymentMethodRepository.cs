using System.Collections.Generic;
using System.Threading.Tasks;
using customerportalapi.Entities;

namespace customerportalapi.Repositories.interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethodCRM> GetPaymentMethod(string storeId);

        Task<PaymentMethodCRM> GetPaymentMethodByCard(string storeId);

        Task<PaymentMethodCRM> GetPaymentMethodByBankAccount(string storeId);

        Task<PaymentMethodCRM> GetPaymentMethodbyId(string paymentMethodId);

    }
}
