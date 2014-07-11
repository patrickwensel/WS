using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS.App.Internet.Objects
{
    public class InvoicePDFRequest
    {
        public string SecurityToken { get; set; }
        public int[] Invoices { get; set; }
    }
}