using AS.App.PassportAddOn.ViewModels.EmployeeSearch;
using AutoMapper;
using WS.Framework.Objects.LDAP;

namespace AS.App.PassportAddOn.Infrastructure
{
    public class AutoMapperBootStrapper
    {
        public static void BootStrap()
        {
            Mapper.CreateMap<Country, CountryViewModel>();
            Mapper.CreateMap<Service, ServiceViewModel>();
            Mapper.CreateMap<Site, SiteViewModel>();
        }
    }
}