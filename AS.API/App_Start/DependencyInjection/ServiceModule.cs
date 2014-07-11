using Autofac;
using WS.Framework.ServicesInterface;
using Module = Autofac.Module;

namespace AS.API.App_Start.DependencyInjection
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WS.Framework.ServicesInterfaceImplementation.LDAPService>().As<ILDAPService>();

            base.Load(builder);
        }
    }
}