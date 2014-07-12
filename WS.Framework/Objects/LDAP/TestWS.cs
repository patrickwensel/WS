using System.Collections.Generic;
using LinqToLdap.Mapping;

namespace WS.Framework.Objects.LDAP
{
    [DirectorySchema("ou=GroupesUtilisateurs,o=algeco,c=com", ObjectClass = "top")]
    class TestWS
    {
        [DirectoryAttribute("description", ReadOnly = true)]
        public string Description { get; set; }

        [DirectoryAttribute("member", ReadOnly = true)]
        public List<string> Member { get; set; }
    }
}
