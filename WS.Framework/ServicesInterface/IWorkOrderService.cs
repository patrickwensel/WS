using WS.Framework.Objects;
using WS.Framework.Objects.MobileWorkOrder;


namespace WS.Framework.ServicesInterface
{
    public interface IWorkOrderService
    {
        WorkOrderResponse AddUpdateWorkOrder(MobileWorkOrder mobileWorkOrder);
        void ProcessMWOImages(string folderPath);
        void ProcessImagesWithoutCheck(string folderPath);
        //WorkOrder GetWorkOrderByID(int workOrderID);
        string RunJDEBusinessFunctionCallWOFunctionByWorkOrderID(string workOrderID);
    }
}
