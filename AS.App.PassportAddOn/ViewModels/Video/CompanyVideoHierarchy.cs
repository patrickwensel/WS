using System.Collections.Generic;

namespace AS.App.PassportAddOn.ViewModels.Video
{
    public class CompanyVideoHierarchy
    {
        public IEnumerable<SBUVideoHierarchy> SBUVideoHierarchies { get; set; }
        public IEnumerable<AreaViewModel> AreaViewModels { get; set; } 
    }
}