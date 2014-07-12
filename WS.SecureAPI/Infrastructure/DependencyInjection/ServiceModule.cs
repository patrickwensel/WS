using Autofac;
using WS.Framework.ServicesInterface;

namespace WS.SecureAPI.Infrastructure.DependencyInjection
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Framework.ServicesInterfaceImplementation.SalesForceService>().As<ISalesForceService>();

            base.Load(builder);
        }
    }
}