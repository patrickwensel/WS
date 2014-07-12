namespace WS.Framework.Objects
{
    public class RequestQuote
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ProductType { get; set; }
        public string LocationZip { get; set; }
        public string WhenNeeded { get; set; }
        public string TimeNeeded { get; set; }
        public string Industry { get; set; }
        public string AdditionalInfo { get; set; }
        public bool NewsUpdates { get; set; }

    }
}
