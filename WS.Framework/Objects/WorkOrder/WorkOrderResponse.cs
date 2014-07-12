using System.Collections.Generic;
using WS.Framework.Objects.Enums;

namespace WS.Framework.Objects
{
    public class WorkOrderResponse
    {
        public WorkOrderType WorkOrderType { get; set; }
        public int WorkOrderID { get; set; }
        public bool Success { get; set; }
        public int ReturnCode { get; set; }
        public List<WorkOrderMessage> WorkOrderMessages { get; set; }
    }
}
