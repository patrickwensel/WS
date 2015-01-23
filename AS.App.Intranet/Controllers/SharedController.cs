using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using NLog;
using WS.Framework.Objects.ActiveDirectory;
using WS.Framework.ServicesInterface;

namespace AS.App.Intranet.Controllers
{
    public class SharedController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private readonly ILDAPService _ldapService;

        public SharedController(ILDAPService ldapService)
        {
            _ldapService = ldapService;
        }

        public ActionResult _LoggedInAs()
        {
            User user = new User();

            try
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                WindowsPrincipal principal = (WindowsPrincipal) Thread.CurrentPrincipal;
                //String adDomainUserName = principal.Identity.Name;

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, principal.Identity.Name);
                    user.FirstName = up.GivenName;
                    user.LastName = up.Surname;
                    user.Email = up.EmailAddress;
                    user.UserName = (principal.Identity.Name).Split('\\')[1];
                }

               /* if (adDomainUserName != null)
                {
                    string adUserName = adDomainUserName.Split('\\')[1];

                    user = _ldapService.GetUserByUserName(adUserName);
                }*/
            }
            catch (Exception e)
            {
                logger.Error("Exception", e);
            }

            Session["user"] = user;

            return View(user);
        }

        
    }
}
