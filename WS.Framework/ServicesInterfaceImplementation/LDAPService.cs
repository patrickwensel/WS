using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using LinqToLdap;
using WS.Framework.Objects.LDAP;
using System.DirectoryServices;
using NLog;
using WS.Framework.ServicesInterface;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class LDAPService : ILDAPService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public User GetUserByUserName(string userName)
        {
            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            User user = new User();

            using (LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                using (var context = new DirectoryContext(connection))
                {

                    user = context.Query<User>().Where(u => u.UserName == userName).FirstOrDefault();
                }
            }

            return user;

        }

        public List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>();

            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            using (LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                using (var context = new DirectoryContext(connection))
                {
                    countries = context.Query<Country>()
                        .Where(u => u.Name != null)
                        .ToList();
                }
            }
            
            return countries;
        }

        public List<Service> GetAllServices()
        {
            List<Service> services = new List<Service>();

            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            using (LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                using (var context = new DirectoryContext(connection))
                {
                    services = context.Query<Service>()
                        .Where(u => u.Name != null)
                        .ToList();
                }
            }

            return services;
        }

        public List<Site> GetAllSites()
        {
            List<Site> sites = new List<Site>();

            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            using (LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                using (var context = new DirectoryContext(connection))
                {
                    sites = context.Query<Site>()
                        .Where(u => (u.Name != null) && (u.Code != null))
                        .ToList();
                }
            }

            return sites;
        }

        public IEnumerable<string> GetAllEmployeeNames()
        {
            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            DirectoryEntry directoryEntry = new DirectoryEntry(domain, serviceUser, servicePassword,
                                                               AuthenticationTypes.ServerBind);
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);

            SearchResultCollection searchResultCollection = directorySearcher.FindAll();

            int count = searchResultCollection.Count;

            List<string> names = new List<string>(); 

            if (count == 1)
            {
                foreach (SearchResult searchResult in searchResultCollection)
                {
                    names.Add(searchResult.Properties.Contains("sn")
                                  ? (searchResult.Properties["sn"][0] == null
                                         ? string.Empty
                                         : searchResult.Properties["sn"][0].ToString())
                                  : string.Empty);
                    
                }
            }

            return names;

        }

        public User WSTest()
        {
            string domain = ConfigurationManager.AppSettings["LDAPDomain"];
            string serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
            string servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

            LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(domain);
            NetworkCredential myCredentials = new NetworkCredential(serviceUser, servicePassword);

            using (LdapConnection connection = new LdapConnection(ldapDirectoryIdentifier, myCredentials, AuthType.Basic))
            {
                connection.SessionOptions.ProtocolVersion = 3;
                using (var context = new DirectoryContext(connection))
                {
                    //var x = context.Query<TestWS>().FirstOrDefault();

                    var z = context.Query<TestWS>().Where(u => u.Description == "TEST_WS");

                    //user = context.Query<User>().Where(u => u.UserName == userName).FirstOrDefault();
                }
            }

            return null;

        }

    }
}

