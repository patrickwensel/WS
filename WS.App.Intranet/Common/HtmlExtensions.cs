using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Specialized;

namespace WS.App.Tools.Common
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString MenuItem(this HtmlHelper helper, 
            string linkText, string actionName, string controllerName)
        {
            string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];

            var builder = new TagBuilder("li");
            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) 
                && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
                builder.AddCssClass("selected");
            builder.InnerHtml = helper.ActionLink(linkText, actionName, controllerName).ToHtmlString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper,
            string linkText, string action, object routeValues)
        {
            var newRoute = routeValues == null
                ? htmlHelper.ViewContext.RouteData.Values
                : new RouteValueDictionary(routeValues);

            newRoute = htmlHelper.ViewContext.HttpContext.Request.QueryString
                .ToRouteDic(newRoute);

            return HtmlHelper.GenerateLink(htmlHelper.ViewContext.RequestContext,
                htmlHelper.RouteCollection, linkText, null,
                action, null, newRoute, null).ToMvcHtml();
        }

        public static MvcHtmlString ToMvcHtml(this string content)
        {
            return MvcHtmlString.Create(content);
        }

        public static RouteValueDictionary ToRouteDic(this NameValueCollection collection)
        {
            return collection.ToRouteDic(new RouteValueDictionary());
        }

        public static RouteValueDictionary ToRouteDic(this NameValueCollection collection,
            RouteValueDictionary routeDic)
        {
            foreach (string key in collection.Keys)
            {
                if (!routeDic.ContainsKey(key))
                    routeDic.Add(key, collection[key]);
            }
            return routeDic;
        }
    }
}