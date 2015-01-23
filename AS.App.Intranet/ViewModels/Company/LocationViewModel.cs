using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AS.App.Intranet.ViewModels.Company
{
    public class LocationViewModel
    {
        public decimal ID { get; set; }
        public decimal LocationStatusID { get; set; }
        public decimal CurrencyID { get; set; }
        public decimal CountryID { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}