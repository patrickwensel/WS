using System.Collections.Generic;
using WS.Framework.WSJDEData;


namespace WS.Framework.ServicesInterface
{
    public interface ISecurityLevelService
    {
        List<SecurityLevel> GetSecurityLevelsByApplicationID(int applicationID);
    }
}
