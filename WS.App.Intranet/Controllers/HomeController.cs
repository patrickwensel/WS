using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace WS.App.Intranet.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            try
            {
                ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
                
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);

                //LogEventInfo theEvent = new LogEventInfo(LogLevel.Debug, "XXXXX", "Pass my custom value");
                //theEvent.Properties["ApplicationID"] = "11";
                //logger.Log(theEvent);

            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
