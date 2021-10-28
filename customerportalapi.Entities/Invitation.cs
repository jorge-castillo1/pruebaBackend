using customerportalapi.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace customerportalapi.Entities
{
    public class Invitation
    {
        public string Dni { get; set; }

        public string Fullname { get; set; }

        public string Email { get; set; }

        public string Language { get; set; }

        public string CustomerType { get; set; }

        /// <summary>
        /// enum InviteInvocationType
        /// </summary>
        public int InvokedBy { get; set; }
    }
}
