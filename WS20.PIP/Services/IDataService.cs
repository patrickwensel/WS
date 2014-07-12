using System.Collections.Generic;
using System.Data;
using PIP.Objects;

namespace PIP.Services
{
    public interface IDataService
    {
        List<People> GetPeopleList();
        List<Branch> GetBranchList();
        PortalUser GetPortalUser();
    }
}
