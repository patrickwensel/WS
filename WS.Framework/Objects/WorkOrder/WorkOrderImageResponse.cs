using System.Collections.Generic;

namespace WS.Framework.Objects.WorkOrder
{
    public class WorkOrderImageResponse
    {
        public int WorkOrderID { get; set; }
        public bool Success { get; set; }
        public int ReturnCode { get; set; }
        public List<WorkOrderMessage> WorkOrderMessages { get; set; }
    }
}
