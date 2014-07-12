using System.Collections.Generic;
using WS.Framework.Objects;
using Attribute = WS.Framework.Objects.WorkOrder.Attribute;

namespace WS.Framework.ServicesInterface
{
    public interface IUserDefinedCodeService
    {
        List<UserDefinedCodeWorkOrder> GetUserDefinedCodeWorkOrder();
        List<Attribute> GetAllAttibutes();
    }
}
