using System.Collections.Generic;
using WS.Framework.Objects;
using WS.Framework.Objects.WorkOrder;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface IUnitService
    {
        Unit GetUnitByUnitNumber(string unitNumber);
        List<UnitAttribute> GetUnitAttributesByAssetID(WSJDE context, int assetID);
        List<OMBOrder> GetLast2OMBOrdersByUnitNumber(string unitNumber);
    }
}
