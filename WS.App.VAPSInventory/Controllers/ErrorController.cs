using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.App.VAPSInventory.ViewModels;

namespace WS.App.VAPSInventory.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

    }
}
