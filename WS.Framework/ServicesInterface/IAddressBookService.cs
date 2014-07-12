using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface IAddressBookService
    {
        string GetNameByID(WSJDE context, int id);
        string GetNameByID(int id);
        string GetBusinessUnitByID(int id);
        //string GetPostalCodeByID(WSJDE context, int addressNumberParent);
        //string GetPostalCodeByID(int addressNumberParent);
    }
}
