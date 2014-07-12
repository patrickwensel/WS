using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using WS.Framework.Objects.Security;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface ISecurityService
    {
        bool IsThisTheCurrentVersion(string versionNumber);
        SecurityTablet GetSecurityTabletByEmployeeNumber(string employeeNumber, string password);
        string GetADUserNameByEmployeeNumber(string employeeNumber);
        string RSADecrypt(string data);
        string RSAEncrypt(string data);
        UserPrincipal GetUserPrincipal();
        string GetUserName();
        string GetDomainAndUsername();
        List<GroupPrincipal> GetGroups(string userName);
        List<GroupPrincipal> GetGroups();
        List<Security> GetSecuritiesByEmployeeID(string employeeID);
        Security AddSecurity(Security security);
        void DeleteSecurity(int securityID);
        void UpdateSecurity(Security security);
        string Encrypt(string value);
        string Decrypt(string value);
        string Encrypt(string value, string passPhrase);
        string Decrypt(string value, string passPhrase);

    }
}
