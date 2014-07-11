using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS.App.Intranet.ViewModels.Employee
{
    public class SecurityViewModel
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string Password { get; set; }
        public int? SecurityLevel { get; set; }
        public decimal ActiveSession { get; set; }
        public string ChangeFlag { get; set; }
        public decimal? ApplicationID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string CreateID { get; set; }
        public string MaintenanceID { get; set; }
    }
}