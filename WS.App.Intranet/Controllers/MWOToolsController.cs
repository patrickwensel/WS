using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.App.Intranet.ViewModels;
using WS.Framework.ServicesInterface;

namespace WS.App.Intranet.Controllers
{
    public class MWOToolsController : Controller
    {
        private readonly IWorkOrderService _workOrderService;

        public MWOToolsController(IWorkOrderService workOrderService)
        {
            _workOrderService = workOrderService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(MWOToolsViewModel mwoToolsViewModel)
        {
            if (ModelState.IsValid)
            {
                string response = _workOrderService.RunJDEBusinessFunctionCallWOFunctionByWorkOrderID(mwoToolsViewModel.WorkOrderNumber.ToString());

                TempData["response"] = response;

                return RedirectToAction("BFConfirm");
            }
            return Index();
        }

        public ActionResult BFConfirm()
        {
            string response = TempData["response"] as string;

            ViewBag.Message = response;
            return View();
        }

    }
}
