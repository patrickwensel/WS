using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface IOMBWorkOrderService
    {
        OMBWorkOrder GetOMBWorkOrderByWorkOrderID(int workOrderID);
        OMBWorkOrder GetOMBWorkOrderByWorkOrderID(int workOrderID, WSJDE context);
    }
}
