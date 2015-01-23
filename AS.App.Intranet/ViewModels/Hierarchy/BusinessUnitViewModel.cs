using System.Collections.Generic;

namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class BusinessUnitViewModel
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        public List<BranchViewModel> BranchViewModels { get; set; }
    }
}