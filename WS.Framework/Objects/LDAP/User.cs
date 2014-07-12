using System;
using System.Collections.Generic;
using LinqToLdap.Mapping;

namespace WS.Framework.Objects.LDAP
{
    [DirectorySchema("ou=Utilisateurs,o=algeco,c=com", ObjectClass = "person")]
    public class User
    {
        [DirectoryAttribute("mail", ReadOnly = true)]
        public string Email { get; set; }

        [DirectoryAttribute("sn", ReadOnly = true)]
        public string LastName { get; set; }

        [DirectoryAttribute("telephonenumber", ReadOnly = true)]
        public string TelephoneNumber { get; set; }

        [DirectoryAttribute("libellesite", ReadOnly = true)]
        public string Location { get; set; }

        [DirectoryAttribute("libellepays", ReadOnly = true)]
        public string Country { get; set; }

        [DirectoryAttribute("dateentree", ReadOnly = true)]
        public string EntryDate { get; set; }

        [DirectoryAttribute("codepays", ReadOnly = true)]
        public string CountryCode { get; set; }

        [DirectoryAttribute("pnptelephone", ReadOnly = true)]
        public string Extension { get; set; }

        [DirectoryAttribute("cn", ReadOnly = true)]
        public string UserName { get; set; }

        [DirectoryAttribute("businesscategory", ReadOnly = true)]
        public string PositionTitle { get; set; }

        [DirectoryAttribute("manager2", ReadOnly = true)]
        public string Manager { get; set; }

        [DirectoryAttribute("codesite", ReadOnly = true)]
        public string CodeSite { get; set; }

        [DirectoryAttribute("givenname", ReadOnly = true)]
        public string FirstName { get; set; }

        [DirectoryAttribute("libellepays", ReadOnly = true)]
        public string GlobalEmployeeCode { get; set; }

        [DirectoryAttribute("libellesociete", ReadOnly = true)]
        public string Company { get; set; }

        [DirectoryAttribute("service", ReadOnly = true)]
        public string FunctionalArea { get; set; }

        [DirectoryAttribute("facsimiletelephonenumber", ReadOnly = true)]
        public string FaxNumber { get; set; }

        [DirectoryAttribute("CompteActif", ReadOnly = true)]
        public string Status { get; set; }

        public List<UserGroup> UserGroups { get; set; }
        public SafetyIncidentAccess SafetyIncidentAccess { get; set; }
        public HoursWorkedAccess HoursWorkedAccess { get; set; }
    }
}

