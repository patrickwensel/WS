using System;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using WS.App.VAPSInventory.ViewModels.Shared;

namespace WS.App.VAPSInventory.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult _LoggedInAs()
        {
            _LoggedInAsViewModel loggedInAsViewModel = new _LoggedInAsViewModel();


            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            if (userName != null)
            {
                string domainName = userName.Split('\\')[0];

                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);

                    loggedInAsViewModel.FirstName = userPrincipal != null ? userPrincipal.GivenName : "????";
                    loggedInAsViewModel.LastName = userPrincipal != null ? userPrincipal.Surname : "????";
                }
            }
            return View(loggedInAsViewModel);
        }
    }
}
