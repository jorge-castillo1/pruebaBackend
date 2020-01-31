using System;

namespace customerportalapi.Entities
{
    public class Token
    {
        public string AccesToken { get; set; }

        public string RefreshToken { get; set; }

        public string Scope { get; set; }

        public string IdToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }

    }
}
