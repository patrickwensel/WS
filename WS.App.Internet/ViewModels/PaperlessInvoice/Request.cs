using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WS.Framework.Validators;

namespace WS.App.Internet.ViewModels.PaperlessInvoice
{
    public class Request
    {
        [Required]
        [DisplayName("First Name:")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name:")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Email format is not valid.")]
        [DisplayName("Email Address:")]
        public string RequesterEmail { get; set; }
        
        [Required]
        [DisplayName("Phone Number:")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumer { get; set; }

        [Required]
        [DisplayName("Account# (found on your invoice):")]
        public string AccountNumber { get; set; }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Email format is not valid.")]
        [DisplayName("Enter e-mail address to use for paperless invoicing:")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Email format is not valid.")]
        [DisplayName("Confirm e-mail address to use for paperless invoicing: ")]
        public string ConfirmEmail { get; set; }

        public List<Unit> Units { get; set; }

        public List<int> Orders { get; set; } 


    }
}