using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WS.App.Intranet.ViewModels;
using WS.Framework.ServicesInterface;
using System.DirectoryServices.AccountManagement;

namespace WS.App.Intranet.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ISecurityService _securityService;

        public ProfileController( ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public ActionResult Index()
        {
            ProfileViewModel profileViewModel = new ProfileViewModel();

            profileViewModel = GetProfileViewModel();

            return View(profileViewModel);
        }

        private ProfileViewModel GetProfileViewModel()
        {
            ProfileViewModel profileViewModel = new ProfileViewModel();

            profileViewModel.UserPrincipal = _securityService.GetUserPrincipal();
            profileViewModel.GroupPrincipals = _securityService.GetGroups();

            return profileViewModel;

        }
    }
}
