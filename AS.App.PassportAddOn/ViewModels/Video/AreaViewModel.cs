using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AS.App.PassportAddOn.ViewModels.Video
{
    public class AreaViewModel
    {
        [DisplayName("ID")]
        public decimal ID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        public string Type { get; set; }

        public decimal? CompanyID { get; set; }
        public decimal? StrategicBusinessUnitID { get; set; }
        public decimal? OperationalBusinessUnitID { get; set; }
        public decimal? BusinessUnitID { get; set; }
        public decimal? BranchID { get; set; }
        public decimal? DepotID { get; set; }

        public List<GroupViewModel> GroupViewModels { get; set; }
    }
}