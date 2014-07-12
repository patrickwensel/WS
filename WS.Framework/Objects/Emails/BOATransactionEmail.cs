using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.Objects.Emails
{
    public class BOATransactionEmail
    {
        public string To { get; set; }
        public string ConfirmationNumber { get; set; }
        public string AmountCharged { get; set; }
    }
}
