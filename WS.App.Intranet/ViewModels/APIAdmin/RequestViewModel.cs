using System;
using System.ComponentModel.DataAnnotations;

namespace WS.App.Intranet.ViewModels.APIAdmin
{
    public class RequestViewModel
    {
        public decimal ID { get; set; }
        public decimal APISourceID { get; set; }
        public decimal APIRequestSourceID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }
    }
}