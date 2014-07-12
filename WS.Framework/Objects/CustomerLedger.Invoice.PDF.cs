using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.Objects
{
    public class CustomerLedgerInvoicePDF
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string AddressNumberParent { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public string DocumentType { get; set; }
        public string PayStatusCode { get; set; }
        public string DocumentCompany { get; set; }
        public int FactorSpecialPayee { get; set; }
        public decimal AmountGross { get; set; }
        public decimal AmountTax { get; set; }
        public decimal AmountOpen { get; set; }
    }
}
