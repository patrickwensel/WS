using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using NLog;
using WS.App.Intranet.ViewModels.Shared;


namespace WS.App.Intranet.Controllers
{
    public class SharedController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult _LoggedInAs()
        {
            WSUser wsUser = new WSUser();

            try
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, principal.Identity.Name);
                    wsUser.FirstName = up.GivenName;
                    wsUser.LastName = up.Surname;
                    wsUser.Email = up.EmailAddress;

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            Session["wsUser"] = wsUser;

            return View(wsUser);
        }

    }
}
