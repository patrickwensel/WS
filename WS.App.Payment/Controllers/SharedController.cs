using System;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using WS.App.Payment.ViewModels;
using WS.App.Payment.ViewModels.Shared;
using WS.Framework.ServicesInterface;
using System.DirectoryServices.AccountManagement;

namespace WS.App.Payment.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult _FirstName()
        {
            _FirstNameViewModel _firstNameViewModel = new _FirstNameViewModel();


            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            String userName = principal.Identity.Name;

            if (userName != null)
            {
                string domainName = userName.Split('\\')[0];

                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);

                    _firstNameViewModel.FirstName = userPrincipal != null ? userPrincipal.GivenName : "????????????";
                }
            }
            return View(_firstNameViewModel);
        }
    }
}
