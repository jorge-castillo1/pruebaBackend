using customerportalapi.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace customerportalapi.Repositories.interfaces
{
    public interface IPaymentRepository
    {
        Task<string> ChangePaymentMethodCard(HttpContent content);
        Task<PaymentMethodCardConfirmationResponse> ConfirmChangePaymentMethodCard(PaymentMethodCardConfirmationToken confirmation);
        Task<PaymentMethodGetCardResponse> GetCard(string token);
        Task<PaymentMethodPayInvoiceResponse> PayInvoice(PaymentMethodPayInvoice payInvoice);
        Task<string> PayInvoiceNewCard(PaymentMethodPayInvoiceNewCard payInvoiceNewCard);
    }
}
