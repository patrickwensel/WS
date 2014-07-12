using System.Collections.Generic;

namespace WS.Framework.Objects.Employees
{
    public class EmployeeSearch
    {
        public string Name { get; set; }
        public string EmployeeStatusID { get; set; }
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public string Sorting { get; set; }
        public List<WSJDEData.Employee> Employees { get; set; } 
    }
}
