using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS.Framework.Objects;

namespace WS.Framework.ServicesInterface
{
    public interface IItemService
    {
        List<ItemActivity> GetItemActivitiesByBusinessUnitID(string businessUnitID);
        List<ItemPart> GetItemPartsByBusinessUnitID(string businessUnitID);
        string GetCatagoryGLBySecondItemNumber(string secondItemNumber);
        int GetItemIDBySecondItemNumber(string secondItemNumber);
        string[] GetSecondAndThirdItemNumberByItemID(int itemID);
    }
}
