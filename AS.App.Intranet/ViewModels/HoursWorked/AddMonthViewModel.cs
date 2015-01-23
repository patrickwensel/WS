using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AS.App.Intranet.ViewModels.HoursWorked
{
    public class AddMonthViewModel
    {
        [DisplayName("Country")]
        [Required(ErrorMessage = "Please enter a Country")]
        public decimal? CountryID { get; set; }

        [DisplayName("Year")]
        [Required(ErrorMessage = "Please enter a Year")]
        public int Year { get; set; }

        [DisplayName("Month")]
        [Required(ErrorMessage = "Please enter a Month")]
        public int Month { get; set; }
    }
}
