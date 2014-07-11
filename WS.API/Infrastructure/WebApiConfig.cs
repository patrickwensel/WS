using System.Web.Http;

namespace WS.API.Infrastructure
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MessageHandlers.Add(new CompressContentResponseHandler());

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "{controller}/{action}",
				defaults: new { }
			);
		}
	}
}
