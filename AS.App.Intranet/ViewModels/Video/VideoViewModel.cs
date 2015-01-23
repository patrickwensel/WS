using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AS.App.Intranet.ViewModels.Video
{
    public class VideoViewModel
    {
        public decimal ID { get; set; }

        [Required]
        [DisplayName("Area")]
        public decimal AreaID { get; set; }

        [Required]
        [DisplayName("Group")]
        public decimal GroupID { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("BrightCove ID")]
        public decimal ReferenceID { get; set; }

        public string Type { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedByUser { get; set; }

        public decimal? CompanyID { get; set; }
        public decimal? StrategicBusinessUnitID { get; set; }
        public decimal? OperationalBusinessUnitID { get; set; }
        public decimal? BusinessUnitID { get; set; }
        public decimal? BranchID { get; set; }
        public decimal? DepotID { get; set; }
    }
}