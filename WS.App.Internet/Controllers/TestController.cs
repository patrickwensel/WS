using System;
using System.Linq;
using System.Web.Mvc;
using NLog;
using WS.App.Internet.ViewModels.Test;
using WS.Framework.WSJDEData;

namespace WS.App.Internet.Controllers
{
    public class TestController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            TestViewModel testViewModel = new TestViewModel();
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    var name = (from x in context.AddressBooks
                                where x.ID == 10
                                select x.NameAlpha).FirstOrDefault();

                    testViewModel.Name = name;
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return View(testViewModel);
        }

    }
}
