
using WS.Framework.Objects.Employees;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterface
{
    public interface IEmployeeService
    {
        EmployeeSearch GetEmployeeSearch(EmployeeSearch employeeSearch);
        Employee GetEmployeeByEmployeeNumber(string employeeNumber);
        string GetEmployeeIDByADUserName(string adUserName);
        int GetEmployeeBranchByADUserName(string adUserName);
    }
}
