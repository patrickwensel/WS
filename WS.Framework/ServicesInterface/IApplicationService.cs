using System.Collections.Generic;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface IApplicationService
    {
        List<Application> GetActiveApplications();
    }
}
