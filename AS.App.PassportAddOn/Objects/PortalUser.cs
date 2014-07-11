using System.Collections;

namespace AS.App.PassportAddOn.Objects
{
    public class PortalUser
    {
        public string User { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public Hashtable LanguageHashTable { get; set; }
    }
}