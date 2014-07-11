using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WS.App.Intranet.ViewModels;
using WS.Framework.ServicesInterface;

namespace WS.App.Intranet.Controllers
{
    public class HyperionToolsController : Controller
    {
        private readonly IHyperionService _hyperionService;

        public HyperionToolsController(IHyperionService hyperionService)
        {
            _hyperionService = hyperionService;
        }

        [Authorize(Roles = "Hyperion GL Extract")]
        public ActionResult Index()
        {
            HyperionToolsViewModel hyperionToolsViewModel = new HyperionToolsViewModel();

            return View(hyperionToolsViewModel);
        }

        [HttpPost]
        public ActionResult Index(HyperionToolsViewModel hyperionToolsViewModel)
        {
            if (ModelState.IsValid)
            {
                string response = _hyperionService.RunGLExtract(hyperionToolsViewModel.Month, hyperionToolsViewModel.Year, hyperionToolsViewModel.LedgerType);

                TempData["response"] = response;

                return RedirectToAction("Confirmation");
            }

            return Index();
        }

        public ActionResult Confirmation()
        {
            string response = TempData["response"] as string;

            ViewBag.Message = response;
            return View();

        }
    }
}
