using Kendo.Mvc.UI;

namespace AS.App.PassportAddOn.Objects
{
    public class EmployeeSearchAdditionalData : DataSourceRequest
    {
        public string Keyword { get; set; }
        public string CountryCode1 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode2 { get; set; }
        public string Location { get; set; }
        public string PositionTitle { get; set; }
        public string FunctionalArea { get; set; }
    }
}