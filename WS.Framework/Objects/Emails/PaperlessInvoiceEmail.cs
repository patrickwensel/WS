using System.Collections.Generic;

namespace WS.Framework.Objects.Emails
{
    public class PaperlessInvoiceEmail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RequesterEmail { get; set; }
        public string PhoneNumer { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string Units { get; set; }
    }
}
