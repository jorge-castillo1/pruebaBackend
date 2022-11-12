namespace customerportalapi.Entities.Enums
{
    public enum TokenUpdateTypes
    {
        OK = 0,
        Pending = 1,
        Reversal = 2,
        ReversalPending = 3,
        Transaction_denied_Rejected = 101,
        Transaction_denied_Bank_reference = 102,
        Transaction_denied_Stolen_or_lost_credit_card = 103,
        Transaction_denied_Fraud = 107,
        Transaction_denied_Other_reasons = 108,
        Bank_systems_error = 200,
        Addon_Payments_systems_error = 300,
        Incorrect_content_on_XML_Integration_problem = 508,
        Incorrect_content_on_XML_Customer_data_error = 509,
        Incorrect_content_on_XML_Account_number_incorrect = 510,
        Deactivated_account = 666
    }
}