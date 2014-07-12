using LinqToLdap.Mapping;

namespace WS.Framework.Objects.LDAP
{
    [DirectorySchema("OU=Pays,ou=Organisations,o=algeco,c=com", ObjectClass = "*")]
    public class Country
    {
        [DirectoryAttribute("ou", ReadOnly = true)]
        public string Code { get; set; }

        [DirectoryAttribute("description", ReadOnly = true)]
        public string Name { get; set; }
    }
}
