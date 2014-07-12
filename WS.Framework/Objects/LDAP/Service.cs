using LinqToLdap.Mapping;

namespace WS.Framework.Objects.LDAP
{
    [DirectorySchema("OU=Services,ou=Organisations,o=algeco,c=com", ObjectClass = "*")]
    public class Service
    {
        [DirectoryAttribute("ou", ReadOnly = true)]
        public string Name { get; set; }

    }
}
