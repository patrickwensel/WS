using System.Collections.Generic;
using WS.Framework.Objects;

namespace WS.Framework.ServicesInterface
{
    public interface IBusinessUnitService
    {
        List<BusinessUnitShort> GetAllBusinessUnitShorts();
    }
}
