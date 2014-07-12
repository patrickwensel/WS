using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Objects;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using WS.Framework.Objects.Security;
using WS.Framework.ServicesInterface;
using NLog;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class SecurityService : ISecurityService
    {
        private const string PrivateKey =
            "<RSAKeyValue><Modulus>s6lpjspk+3o2GOK5TM7JySARhhxE5gB96e9XLSSRuWY2W9F951MfistKRzVtg0cjJTdSk5mnWAVHLfKOEqp8PszpJx9z4IaRCwQ937KJmn2/2VyjcUsCsor+fdbIHOiJpaxBlsuI9N++4MgF/jb0tOVudiUutDqqDut7rhrB/oc=</Modulus><Exponent>AQAB</Exponent><P>3J2+VWMVWcuLjjnLULe5TmSN7ts0n/TPJqe+bg9avuewu1rDsz+OBfP66/+rpYMs5+JolDceZSiOT+ACW2Neuw==</P><Q>0HogL5BnWjj9BlfpILQt8ajJnBHYrCiPaJ4npghdD5n/JYV8BNOiOP1T7u1xmvtr2U4mMObE17rZjNOTa1rQpQ==</Q><DP>jbXh2dVQlKJznUMwf0PUiy96IDC8R/cnzQu4/ddtEe2fj2lJBe3QG7DRwCA1sJZnFPhQ9svFAXOgnlwlB3D4Gw==</DP><DQ>evrP6b8BeNONTySkvUoMoDW1WH+elVAH6OsC8IqWexGY1YV8t0wwsfWegZ9IGOifojzbgpVfIPN0SgK1P+r+kQ==</DQ><InverseQ>LeEoFGI+IOY/J+9SjCPKAKduP280epOTeSKxs115gW1b9CP4glavkUcfQTzkTPe2t21kl1OrnvXEe5Wrzkk8rA==</InverseQ><D>HD0rn0sGtlROPnkcgQsbwmYs+vRki/ZV1DhPboQJ96cuMh5qeLqjAZDUev7V2MWMq6PXceW73OTvfDRcymhLoNvobE4Ekiwc87+TwzS3811mOmt5DJya9SliqU/ro+iEicjO4v3nC+HujdpDh9CVXfUAWebKnd7Vo5p6LwC9nIk=</D></RSAKeyValue>";

        private const string PublicKey =
            "<RSAKeyValue><Modulus>s6lpjspk+3o2GOK5TM7JySARhhxE5gB96e9XLSSRuWY2W9F951MfistKRzVtg0cjJTdSk5mnWAVHLfKOEqp8PszpJx9z4IaRCwQ937KJmn2/2VyjcUsCsor+fdbIHOiJpaxBlsuI9N++4MgF/jb0tOVudiUutDqqDut7rhrB/oc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string Salt = "tu89geji340t89u2";

        private const int Keysize = 256;

        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IEmployeeService _employeeService;
        private readonly IOracleHelperService _oracleHelperService;

        public SecurityService(IEmployeeService employeeService, IOracleHelperService oracleHelperService)
        {
            _employeeService = employeeService;
            _oracleHelperService = oracleHelperService;
        }

        public bool IsThisTheCurrentVersion(string versionNumber)
        {
            bool isCurrentVersion = true;

            string currentVersion = ConfigurationManager.AppSettings.Get("VersionNumber");

            string[] digits = versionNumber.Split(new[] { '.' });

            versionNumber = digits[0] + "." + digits[1] + "." + digits[2];

            if (currentVersion != versionNumber)
            {
                isCurrentVersion = false;
            }

            return isCurrentVersion;
        }

        public SecurityTablet GetSecurityTabletByEmployeeNumber(string employeeNumber, string password)
        {
            SecurityTablet securityTablet = new SecurityTablet();

            try
            {
                using (WSJDE context = new WSJDE())
                {

                    var employees = (from e in context.Employees
                                     join ws in context.Securities on e.ID equals
                                         ws.EmployeeID
                                     join wa in context.Applications on ws.ApplicationID
                                         equals wa.ID
                                     where e.EmployeeStatusID == DbFunctions.AsNonUnicode("A")
                                           && ws.SecurityLevelLevelID == 1
                                           && e.EmployeeNumber == DbFunctions.AsNonUnicode(employeeNumber)
                                           && ws.ApplicationID == 11
                                     select new
                                         {
                                             e.ReportLocationID,
                                             ws.Password,
                                             e.EmployeeNumber
                                         });

                    List<EmployeeInfo> employeeInfos =
                        employees.Select(
                            employee =>
                            new EmployeeInfo
                                {
                                    ReportLocationID = employee.ReportLocationID,
                                    EmployeeNumber = employee.EmployeeNumber,
                                    Password = employee.Password
                                }).ToList();

                    int reportLocationNumericCount = employeeInfos.Count();


                    if (reportLocationNumericCount == 1)
                    {
                        string employeePassword = employeeInfos[0].Password.Trim();

                        if (employeePassword == password)
                        {
                            securityTablet.AuthenticationCode = 1;
                            securityTablet.ReportLocationNumber = employeeInfos[0].ReportLocationID.Trim();
                        }

                        else
                        {
                            securityTablet.AuthenticationCode = 2;
                            securityTablet.ReportLocationNumber = null;
                            securityTablet.AuthenticationCodeMessage =
                                ConfigurationManager.AppSettings.Get("AuthenticationCode1Message");
                            logger.Info("Attempt to login but password was incorrect. Employee #: " + employeeNumber +
                                         " Password: " + password);
                        }
                    }

                    if (reportLocationNumericCount == 0)
                    {
                        securityTablet.AuthenticationCode = 2;
                        securityTablet.ReportLocationNumber = null;
                        securityTablet.AuthenticationCodeMessage =
                            ConfigurationManager.AppSettings.Get("AuthenticationCode2Message");
                        logger.Info("Attempt to login but user did not exist. Employee #: " + employeeNumber +
                                     " Password: " + password);

                    }

                    if (reportLocationNumericCount > 1)
                    {
                        securityTablet.AuthenticationCode = 3;
                        securityTablet.ReportLocationNumber = null;
                        securityTablet.AuthenticationCodeMessage =
                            ConfigurationManager.AppSettings.Get("AuthenticationCode3Message");

                        logger.Error(
                            "More than one user was returned by GetSecurityTabletByEmployeeNumber. Employee #: " +
                            employeeNumber + " Password: " + password);
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
                securityTablet.AuthenticationCode = 4;
                securityTablet.ReportLocationNumber = null;
                securityTablet.AuthenticationCodeMessage =
                    ConfigurationManager.AppSettings.Get("AuthenticationCode4Message");
            }

            return securityTablet;
        }

        public string GetADUserNameByEmployeeNumber(string employeeNumber)
        {
            string adUserName = "";

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    adUserName = (from e in context.Employees
                                  where e.EmployeeNumber.Trim() == DbFunctions.AsNonUnicode(employeeNumber)
                                  select e.ADUserName).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return adUserName;
        }

        public string RSADecrypt(string data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string[] dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(PrivateKey);
            byte[] decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public string RSAEncrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        //public UserPrincipal GetUserPrincipal()
        //{
        //    WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        //    if (windowsIdentity != null)
        //    {
        //        string userName = windowsIdentity.Name;
        //        string domainName = userName.Split('\\')[0];

        //        using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName))
        //        {
        //            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);
        //            return userPrincipal;
        //        }
        //    }

        //    return null;
        //}

        //public string GetUserName()
        //{
        //    string userName = null;

        //    WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        //    if (windowsIdentity != null)
        //    {
        //        string userNameWithDomain = windowsIdentity.Name;
        //        userName = userNameWithDomain.Split('\\')[1];
        //    }

        //    return userName;
        //}

        public UserPrincipal GetUserPrincipal()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            if (userName != null)
            {
                string domainName = userName.Split('\\')[0];

                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);
                    if (userPrincipal != null)
                    {
                        return userPrincipal;
                    }
                }
            }

            return null;

        }

        public string GetUserName()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            if (userName != null)
            {
                return userName.Split('\\')[1];
            }

            return null;
        }

        public string GetDomainAndUsername()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            if (userName != null)
            {
                return userName;
            }

            return null;
        }

        public List<GroupPrincipal> GetGroups(string userNameWithDomain)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userNameWithDomain);

            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                result.AddRange(groups.OfType<GroupPrincipal>());
            }

            return result;
        }

        public List<GroupPrincipal> GetGroups()
        {
            List<GroupPrincipal> groupPrincipals = new List<GroupPrincipal>();
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                string userNameWithDomain = windowsIdentity.Name;
                groupPrincipals = GetGroups(userNameWithDomain);

            }

            return groupPrincipals;
        }

        public List<Security> GetSecuritiesByEmployeeID(string employeeID)
        {
            List<Security> securities = new List<Security>();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    string userName = HttpContext.Current.User.Identity.Name.Split('\\')[1];

                    string userEmployeeID = _employeeService.GetEmployeeIDByADUserName(userName);

                    List<int?> userSecurities = (from s in context.Securities
                                                       where s.EmployeeID == userEmployeeID
                                                             && s.ApplicationID == 8
                                                 select s.SecurityLevelLevelID).ToList();

                    if (userSecurities.Count != 0)
                    {

                        if (userSecurities.Contains(1))
                        {
                            securities = (from s in context.Securities
                                          join a in context.Applications on s.ApplicationID equals a.ID
                                          where s.EmployeeID == DbFunctions.AsNonUnicode(employeeID)
                                          orderby a.Name
                                          select s).ToList();
                        }
                        else
                        {
                            var apps1 = (from s in context.Securities
                                         where s.EmployeeID == DbFunctions.AsNonUnicode(userEmployeeID)
                                               && s.ApplicationID == 8
                                         select s.SecurityLevelLevelID);

                            var apps2 = (from sl in context.SecurityLevels
                                         where sl.ApplicationID == 8
                                         && apps1.Contains(Convert.ToInt32(sl.LevelID))
                                         select sl.SubApplicationID);

                            securities = (from s in context.Securities
                                          join a in context.Applications on s.ApplicationID equals a.ID
                                          where s.EmployeeID == DbFunctions.AsNonUnicode(employeeID)
                                          && apps2.Contains(s.ApplicationID)
                                          orderby a.Name
                                          select s).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return securities;
        }

        public Security AddSecurity(Security security)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    int countOfExistingSecurities = (from s in context.Securities
                                                     where s.EmployeeID == security.EmployeeID
                                                           && s.ApplicationID == security.ApplicationID
                                                           && s.SecurityLevelLevelID == security.SecurityLevelLevelID
                                                     select s).Count();
                    if (countOfExistingSecurities == 0)
                    {

                        string userName = " ";

                        if (HttpContext.Current.User.Identity.Name != null)
                        {
                            userName = HttpContext.Current.User.Identity.Name.Split('\\')[1];
                        }

                        DateTime now = DateTime.Now;

                        if (security.Password == "0000" || String.IsNullOrWhiteSpace(security.Password))
                        {
                            Random random = new Random();
                            int passworInt = random.Next(1, 9999);
                            string password = passworInt.ToString().PadLeft(4, Convert.ToChar("0"));
                            security.Password = password;
                        }

                        security.ChangeFlag = "N";
                        security.CreateDate = now;
                        security.MaintenanceDate = now;
                        security.CreateID = userName;
                        security.MaintenanceID = userName;

                        context.Securities.Add(security);
                        context.SaveChanges();
                    }
                    else
                    {
                        security = null;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return security;
        }

        public void DeleteSecurity(int securityID)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    Security rowToDelete = (from s in context.Securities
                                       where s.ID == securityID
                                       select s).First();
                    
                    if (rowToDelete != null)
                    {
                        context.Securities.Remove(rowToDelete);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

        }

        public void UpdateSecurity(Security security)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    string userName = " ";

                    if (HttpContext.Current.User.Identity.Name != null)
                    {
                        userName = HttpContext.Current.User.Identity.Name.Split('\\')[1];
                    }

                    DateTime now = DateTime.Now;
                    Security rowToUpdate = (from s in context.Securities
                                            where s.ID == security.ID
                                            select s).First();

                    if (rowToUpdate != null)
                    {
                        rowToUpdate.ApplicationID = security.ApplicationID;
                        rowToUpdate.SecurityLevelLevelID = security.SecurityLevelLevelID;
                        rowToUpdate.Password = security.Password;
                        rowToUpdate.MaintenanceID = userName;
                        rowToUpdate.MaintenanceDate = now;


                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

        }

        public string Encrypt(string value)
        {
            return Encrypt(value, "W1ll$c0t9O1SB0nd");
        }

        public string Decrypt(string value)
        {
            return Decrypt(value, "W1ll$c0t9O1SB0nd");
        }

        public string Encrypt(string value, string passPhrase)
        {
            byte[] saltByte = Encoding.UTF8.GetBytes(Salt);
            byte[] valueByte = Encoding.UTF8.GetBytes(value);
            
            //PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltByte, 1000);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, saltByte);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(valueByte, 0, valueByte.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string value, string passPhrase)
        {
            byte[] saltByte = Encoding.ASCII.GetBytes(Salt);
            byte[] valueByte = Convert.FromBase64String(value);

            //PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltByte, 1000);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, saltByte);
            MemoryStream memoryStream = new MemoryStream(valueByte);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] newValueByte = new byte[valueByte.Length];
            int decryptedByteCount = cryptoStream.Read(newValueByte, 0, newValueByte.Length);
            
            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.UTF8.GetString(newValueByte, 0, decryptedByteCount);
        }

    }

    internal class EmployeeInfo
    {
        public string ReportLocationID { get; set; }
        public string Password { get; set; }
        public string EmployeeNumber { get; set; }
    }


}
