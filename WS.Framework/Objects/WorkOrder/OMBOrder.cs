using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Framework.Objects.WorkOrder
{
    public class OMBOrder
    {
        public decimal SKey { get; set; }
        public string Name { get; set; }
        public decimal QStatus { get; set; }
        public string Type { get; set; }
        public decimal OMBOrderHeaderID {get; set; }
        public decimal OrderTypeID { get; set; }
    }
}
