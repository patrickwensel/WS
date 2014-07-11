using System;

namespace WS.App.Intranet.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public string NewPassword { get; set; }
        public string Title { get; set; }
        public string EmployeeNumber { get; set; }
        public string CostCenter { get; set; }
        public string ADUserName { get; set; }
        public string Email { get; set; }
        public string EmployeeStatusID { get; set; }
        public string ExemptStatus { get; set; }
        public string ID { get; set; }
        public string ReportLocationID { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? HireDate { get; set; }
        public string DepartmentCode { get; set; }
        public string ReportsToEmployeeNumber { get; set; }
        public string ReportsToEmployeeID { get; set; }
        public string TitleCode { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string EmployeeTypeID { get; set; }
    }
}