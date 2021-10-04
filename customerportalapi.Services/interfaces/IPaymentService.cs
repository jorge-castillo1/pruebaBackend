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

        Task<bool> UpdatePaymentProcess(SignatureStatus value);

        Task<bool> UpdatePaymentCardProcess(SignatureStatus value, Process process);

        Task<string> ChangePaymentMethodCardLoad(PaymentMethodCard paymentMethod);

        Task<bool> ChangePaymentMethodCardResponseAsync(PaymentMethodCardData cardData);

        Task<bool> ChangePaymentMethodCard(PaymentMethodCardSignature paymentMethodCardSignature);
        
        Task<Card> GetCard(string username, string smContractCode);

        Task<PaymentMethodPayInvoiceResponse> PayInvoice(PaymentMethodPayInvoice payInvoice);

        Task<string> PayInvoiceByNewCardLoad(PaymentMethodPayInvoiceNewCard paymentMethod);
        
        Task<bool> PayInvoiceByNewCardResponse(PaymentMethodPayInvoiceNewCardResponse payRes);
        
        Task<string> UpdateCardLoad(PaymentMethodUpdateCardData updateCardData);
        
        Task<bool> UpdateCardResponseAsync(PaymentMethodUpdateCardResponse updateCardResponse);

        Task<List<PaymentMethods>> GetAvailablePaymentMethods(string smContractCode);

    }
}
