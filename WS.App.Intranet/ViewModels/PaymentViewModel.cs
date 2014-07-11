using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Attributes;
using MVCControlsToolkit.DataAnnotations;
using WS.Framework.Validators;

namespace WS.App.Intranet.ViewModels
{
    [Validator(typeof(PaymentValidator))]
    public class PaymentViewModel
    {
        public string AnyInvoicesEntered { get; set; }

        [DisplayName("Payment Method")]
        public int? PaymentMethod { get; set; }

        [DisplayName("Confirmation Email Address")]
        [Email]
        public string ConformationEmailAddress { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Sub-Total")]
        public Decimal SubTotal { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Tax")]
        public Decimal TaxTotal { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Total Payment")]
        public Decimal? PaymentTotal { get; set; }

        public List<InvoiceViewModel> InvoiceViewModels { get; set; }
        public IEnumerable<SelectListItem> PaymentMethods { get; set; }
    }

    public class PaymentValidator : AbstractValidator<PaymentViewModel>
    {
        public PaymentValidator()
        {
            RuleFor(model => model.ConformationEmailAddress)
                .NotEmpty()
                .When(model => model.PaymentTotal > 0)
                .WithMessage("Confirmation Email Address is required");
            RuleFor(model => model.PaymentMethod)
                .NotEmpty()
                .When(model => model.PaymentTotal > 0)
                .WithMessage("Payment Method is required");
            RuleFor(model => model.PaymentTotal)
                .NotEqual(0)
                .WithMessage("Total Payment must be greater than $0.00");
            RuleFor(model => model.PaymentTotal)
                .LessThan(50000)
                .WithMessage("Total Payment must be less than $50,000.00");
            RuleFor(model => model.AnyInvoicesEntered)
                .NotEmpty()
                .When(AtLeastOneValueEntered)
                .WithMessage("No valid invoices entered");
        }

        private bool AtLeastOneValueEntered(PaymentViewModel model)
        {
            int countOfInvocies = model.InvoiceViewModels.Count(x => x.Number != null);

            if (countOfInvocies == 0)
            {
                return true;
            }
            return false;
        }
    }
}