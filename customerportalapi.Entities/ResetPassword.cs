namespace customerportalapi.Entities
{
    public class ResetPassword
    {
        public string Username { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
