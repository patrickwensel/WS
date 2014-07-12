using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS.App.VAPSInventory.ViewModels
{
    public class ErrorPresentation
    {
        public int StatusCode { get; set; }
        public Exception TheException { get; set; }
    }
}