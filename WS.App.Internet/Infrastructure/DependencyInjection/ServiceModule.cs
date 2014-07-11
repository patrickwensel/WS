using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.App.Internet.Infrastructure.DependencyInjection
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Framework.ServicesInterfaceImplementation.SecurityService>().As<ISecurityService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.UnitService>().As<IUnitService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.BusinessUnitService>().As<IBusinessUnitService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ItemService>().As<IItemService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.HelperService>().As<IHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.JDEService>().As<IJDEService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.WorkOrderService>().As<IWorkOrderService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.CustomerLedgerService>().As<ICustomerLedgerService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmailService>().As<IEmailService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.OracleHelperService>().As<IOracleHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.AddressBookService>().As<IAddressBookService>();

            base.Load(builder);
        }
    }
}