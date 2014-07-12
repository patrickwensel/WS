using WS.Framework.Objects.Enums;

namespace WS.Framework.Objects
{
    public class AccountNumberLookUpResult
    {
        public AccountNumberLookUpResultCode ResultCode { get; set; }
        public string CompanyName { get; set; }
        public int DefaultInvoiceNumber { get; set; }
        public int DefaultTab { get; set; }
        public int AccountNumber { get; set; }
    }
}
