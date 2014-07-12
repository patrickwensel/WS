using WS.Framework.Objects.Enums;

namespace WS.Framework.Objects
{
    public class BOATransactionResult
    {
        public BOATransactionReturnCode Code { get; set; }
        public string PaymentUrl { get; set; }
    }
}