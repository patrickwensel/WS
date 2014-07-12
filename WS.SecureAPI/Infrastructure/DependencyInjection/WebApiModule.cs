using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace WS.SecureAPI.Infrastructure.DependencyInjection
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new HttpContextWrapper(HttpContext.Current))
                   .InstancePerApiRequest()
                   .As<HttpContextBase>();

            builder.RegisterApiControllers(ThisAssembly);
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterWebApiModelBinders(ThisAssembly);
        }
    }
}