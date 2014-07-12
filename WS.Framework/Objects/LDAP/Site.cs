using LinqToLdap.Mapping;

namespace WS.Framework.Objects.LDAP
{
    [DirectorySchema("OU=Sites,ou=Organisations,o=algeco,c=com", ObjectClass = "*")]
    public class Site
    {
        [DirectoryAttribute("ou", ReadOnly = true)]
        public string Code { get; set; }

        [DirectoryAttribute("description", ReadOnly = true)]
        public string Name { get; set; }
    }
}
