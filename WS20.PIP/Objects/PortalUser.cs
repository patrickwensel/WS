using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace PIP.Objects
{
    public class PortalUser
    {
        public string User { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public bool PIPAdmin { get; set; }
        public Hashtable LanguageHashTable { get; set; }
    }
}