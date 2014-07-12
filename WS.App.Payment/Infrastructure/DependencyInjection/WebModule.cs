using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.App.Payment.Infrastructure.DependencyInjection
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Framework.ServicesInterfaceImplementation.CustomerLedgerService>().As<ICustomerLedgerService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.BOAService>().As<IBOAService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmailService>().As<IEmailService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.SecurityService>().As<ISecurityService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.HelperService>().As<IHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.ApplicationLogCodeService>().As<IApplicationLogCodeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.AddressBookService>().As<IAddressBookService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.EmployeeService>().As<IEmployeeService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.OracleHelperService>().As<IOracleHelperService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.AddressBookService>().As<IAddressBookService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.CustomerActivityLogService>().As<ICustomerActivityLogService>();
            builder.RegisterType<Framework.ServicesInterfaceImplementation.MediaObjectStorageService>().As<IMediaObjectStorageService>();

            base.Load(builder);
        }
    }
}