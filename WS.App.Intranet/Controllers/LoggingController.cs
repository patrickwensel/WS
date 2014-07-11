using System.Web.Mvc;
using WS.App.Tools.ViewModels;

namespace WS.App.Intranet.Controllers
{
    public class LoggingController : Controller
    {
        [Authorize(Roles = "Application Logging")]
        public ActionResult Index()
        {
            LoggingViewModel loggingViewModel = new LoggingViewModel();

            return View(loggingViewModel);
        }
    }
}
