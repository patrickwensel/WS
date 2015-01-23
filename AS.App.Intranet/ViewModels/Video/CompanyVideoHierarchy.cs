using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AS.App.Intranet.ViewModels.Video
{
    public class CompanyVideoHierarchy
    {
        public IEnumerable<SBUVideoHierarchy> SBUVideoHierarchies { get; set; }
        public IEnumerable<AreaViewModel> AreaViewModels { get; set; } 
    }
}