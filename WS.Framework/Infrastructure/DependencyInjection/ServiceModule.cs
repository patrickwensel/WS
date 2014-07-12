using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.Framework.Infrastructure.DependencyInjection
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServicesInterfaceImplementation.SecurityService>().As<ISecurityService>();
            builder.RegisterType<ServicesInterfaceImplementation.UnitService>().As<IUnitService>();
            builder.RegisterType<ServicesInterfaceImplementation.BusinessUnitService>().As<IBusinessUnitService>();
            builder.RegisterType<ServicesInterfaceImplementation.ItemService>().As<IItemService>();
            builder.RegisterType<ServicesInterfaceImplementation.HelperService>().As<IHelperService>();
            builder.RegisterType<ServicesInterfaceImplementation.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<ServicesInterfaceImplementation.JDEService>().As<IJDEService>();
            builder.RegisterType<ServicesInterfaceImplementation.WorkOrderService>().As<IWorkOrderService>();
            builder.RegisterType<ServicesInterfaceImplementation.OMBWorkOrderService>().As<IOMBWorkOrderService>();
            builder.RegisterType<ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<ServicesInterfaceImplementation.ProductCategoryService>().As<IProductCategoryService>();

            base.Load(builder);
        }
    }
}