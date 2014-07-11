using System.ComponentModel;

namespace AS.App.PassportAddOn.ViewModels.EmployeeSearch
{
    public class SearchViewModel
    {
        [DisplayName("Keyword")]
        public string Keyword { get; set; }

        [DisplayName("Country")]
        public string CountryCode { get; set; }

        public string Advanced { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("Position/Title")]
        public string PositionTitle { get; set; }

        [DisplayName("Functional Area")]
        public string FunctionalArea { get; set; }

    }
}