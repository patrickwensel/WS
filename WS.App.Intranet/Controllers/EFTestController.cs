using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.Framework;
using WS.Framework.WSJDEData;

namespace WS.App.Intranet.Controllers
{
    public class EFTestController : Controller
    {
        //
        // GET: /EFTest/

        public ActionResult Index()
        {
            using (WSJDE context = new WSJDE())
            {
                var name = (from x in context.UserDefinedCodeTypes
                               select x).FirstOrDefault();
            }


            return View();
        }

    }
}
