using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.API.Infrastructure.DependencyInjection
{
    public class ServiceModule : Module
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
            builder.RegisterType<Framework.ServicesInterfaceImplementation.UserDefinedCodeService>().As<IUserDefinedCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EquipmentService>().As<IEquipmentService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.OMBWorkOrderService>().As<IOMBWorkOrderService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmailService>().As<IEmailService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.OracleHelperService>().As<IOracleHelperService>();

            base.Load(builder);
        }
    }
}