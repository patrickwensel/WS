using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace WS.App.Intranet.ViewModels
{
    public class ProfileViewModel
    {
        public UserPrincipal UserPrincipal { get; set; }
        public List<GroupPrincipal> GroupPrincipals { get; set; }
     
    }
}