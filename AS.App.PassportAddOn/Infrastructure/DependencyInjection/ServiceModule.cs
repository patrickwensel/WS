using Autofac;
using WS.Framework.ServicesInterface;

namespace AS.App.PassportAddOn.Infrastructure.DependencyInjection
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WS.Framework.ServicesInterfaceImplementation.LDAPService>().As<ILDAPService>();

            base.Load(builder);
        }
    }
}