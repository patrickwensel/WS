using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AS.App.Intranet.ViewModels.HoursWorked
{
    public class HoursWorkedViewModel
    {
        public decimal ID { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? Hours { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime? CreatedByDate { get; set; }
        public decimal? CountryID { get; set; }
        public decimal? LocationID { get; set; }
        public string EditedBy { get; set; }

        [Required(ErrorMessage = "Please enter an Entity")]
        public decimal? EntityID { get; set; }
    }
}