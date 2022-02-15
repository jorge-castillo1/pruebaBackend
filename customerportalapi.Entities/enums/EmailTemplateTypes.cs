namespace customerportalapi.Entities.enums
{
    public enum EmailTemplateTypes
    {        
        WelcomeEmailExtended = 0, //recordatorio de invitación
        FormContact = 1,
        FormCall = 2,
        FormOpinion = 3,
        ForgotPassword = 4,
        RequestDigitalContract = 5,
        UpdatePaymentMethod = 6,
        RequestDigitalInvoice = 7,
        FormContactCustomer = 8,
        FormCallCustomer = 9,
        EditDataCustomer = 10,
        EditAccessCode = 11,
        InvitationError = 12,
        WelcomeEmailShort = 13, //creación
        ErrorInvitationEmailAlreadyExists = 14,
        ErrorChangeEmailAlreadyExists = 15,
        SaveNewUser,
    }
}
