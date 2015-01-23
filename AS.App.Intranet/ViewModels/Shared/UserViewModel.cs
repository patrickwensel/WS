using System;
using System.Collections.Generic;

namespace AS.App.Intranet.ViewModels.Shared
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string TelephoneNumber { get; set; }
        public string Domain { get; set; }
        public string Department { get; set; }
        public string Country { get; set; }
        public DateTime EntryDate { get; set; }
        public string CountryCode { get; set; }
        public string Extension { get; set; }
        public string Actif { get; set; }
        public string TBD1 { get; set; }
        public string StrategicBusinessUnit { get; set; }
        public string UserName { get; set; }
        public string ADSPath { get; set; }
        public string Title { get; set; }
        public string Manager { get; set; }
        public string CodeSite { get; set; }
        public string KPIRegion { get; set; }
        public string Profile { get; set; }
        public string FirstName { get; set; }
        public string GlobalEmployeeCode { get; set; }
        public string Company { get; set; }
        public string Area { get; set; }
        public string FaxNumber { get; set; }

        public List<UserGroupViewModel> UserGroups { get; set; }
        public SafetyIncidentAccessViewModel SafetyIncidentAccessViewModel { get; set; }
        public HoursWorkedAccessViewModel HoursWorkedAccessViewModel { get; set; }

    }
}