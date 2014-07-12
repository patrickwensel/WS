using System;
using System.Linq;
using NLog;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class OMBWorkOrderService : IOMBWorkOrderService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public OMBWorkOrder GetOMBWorkOrderByWorkOrderID(int workOrderID)
        {
            OMBWorkOrder ombWorkOrder = new OMBWorkOrder();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    ombWorkOrder = GetOMBWorkOrderByWorkOrderID(workOrderID, context);
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return ombWorkOrder;

        }

        public OMBWorkOrder GetOMBWorkOrderByWorkOrderID(int workOrderID, WSJDE context)
        {
            OMBWorkOrder ombWorkOrder = (from owo in context.OMBWorkOrders
                                         where owo.WorkOrderID == workOrderID
                                         select owo).FirstOrDefault();
            return ombWorkOrder;
        }

    }
}
