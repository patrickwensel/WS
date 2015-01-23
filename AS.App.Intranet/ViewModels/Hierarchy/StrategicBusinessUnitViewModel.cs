using System.Collections.Generic;
using WS.Framework.ASJDE;

namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class StrategicBusinessUnitViewModel
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EntityID { get; set; }
        public string Name { get; set; }
        public List<OperationalBusinessUnitViewModel> OBUViewModels { get; set; }

    }
}