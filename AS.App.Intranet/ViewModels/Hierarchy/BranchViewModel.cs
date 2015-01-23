using System.Collections.Generic;
using AS.App.Intranet.ViewModels.Company;

namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class BranchViewModel
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        public List<DepotViewModel> DepotViewModels { get; set; }
    }
}