using System.Collections.Generic;

namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class CompanyViewModel
    {
        public decimal ID { get; set; }
        public decimal EntityID { get; set; }
        public string Name { get; set; }
        public EntityViewModel EntityViewModel { get; set; }
        public List<StrategicBusinessUnitViewModel> StrategicBusinessUnitViews { get; set; }
    }
}