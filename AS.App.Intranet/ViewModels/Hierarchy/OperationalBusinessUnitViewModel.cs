using System.Collections.Generic;

namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class OperationalBusinessUnitViewModel
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal StrategicBusinessUnitID { get; set; }
        public decimal EntityID { get; set; }
        public string Name { get; set; }
        public List<BusinessUnitViewModel> BUViewModels { get; set; }
    }
}