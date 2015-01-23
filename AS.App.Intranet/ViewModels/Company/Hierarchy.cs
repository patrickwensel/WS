using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AS.App.Intranet.ViewModels.Hierarchy;

namespace AS.App.Intranet.ViewModels.Company
{
    public class Hierarchy
    {
        public IEnumerable<CompanyViewModel> CompanyViewModels { get; set; }
    }
}