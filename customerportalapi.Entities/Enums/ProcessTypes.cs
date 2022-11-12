using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities.Enums
{
    public enum ProcessTypes
    {
        PaymentMethodChangeBank = 0,
        PaymentMethodChangeCard = 1,
        PaymentMethodChangeCardSignature = 2,
        Payment = 3
    }
}
