namespace customerportalapi.Entities
{
    public class AccessCode : Token
    {
        public string Password { get; set; }

        public string ContractId { get; set; }
    }
}
