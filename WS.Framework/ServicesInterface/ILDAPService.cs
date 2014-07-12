using System.Collections.Generic;
using WS.Framework.Objects.LDAP;

namespace WS.Framework.ServicesInterface
{
    public interface ILDAPService
    {
        User GetUserByUserName(string userName);
        IEnumerable<string> GetAllEmployeeNames();
        List<Country> GetAllCountries();
        List<Service> GetAllServices();
        List<Site> GetAllSites();
        User WSTest();
    }
}
