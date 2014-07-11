using AutoMapper;
using WS.App.Intranet.ViewModels;
using WS.App.Intranet.ViewModels.Employee;
using WS.Framework.Objects.Security;
using WS.Framework.WSJDEData;

namespace WS.App.Intranet.Infrastructure
{
    public static class AutoMapperBootStrapper
    {
        public static void BootStrap()
        {
            Mapper.CreateMap<Employee, EmployeeViewModel>();
        }
    }
}