using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.App.VAPSInventory.Infrastructure.DependencyInjection
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Framework.ServicesInterfaceImplementation.CustomerLedgerService>().As<ICustomerLedgerService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmailService>().As<IEmailService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.SecurityService>().As<ISecurityService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.HelperService>().As<IHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.AddressBookService>().As<IAddressBookService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.OracleHelperService>().As<IOracleHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.AddressBookService>().As<IAddressBookService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ProductCategoryService>().As<IProductCategoryService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ProductService>().As<IProductService>();

            base.Load(builder);
        }
    }
}