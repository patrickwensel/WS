using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Attributes;
using MVCControlsToolkit.DataAnnotations;

namespace WS.App.Payment.ViewModels
{
    [Validator(typeof(InvoiceValidator))]
    public class InvoiceViewModel
    {
        [DisplayName("Invoice Number")]
        public int? Number { get; set; }

        [DataType(DataType.Date),DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Invoice Date")]
        public DateTime? Date { get; set; }

        public string AddressNumberParent { get; set; }

        public string SupplierInvoiceNumber { get; set; }

        public string DocumentType { get; set; }

        public string PayStatusCode { get; set; }

        public bool OutstandingBalance { get; set; }

        public bool Payable { get; set; }

        public int DisplayType { get; set; }

        public string DisplayMessage { get; set; }

        public string DocumentCompany { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Invoice Amount (Less Tax)")]
        public decimal AmountGross { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Tax Amount")]
        public decimal AmountTax { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Total Amount")]
        public decimal AmountTotal { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Amount to Pay")]
        public Decimal AmountToPayGross { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Tax Amount to Pay")]
        public Decimal AmountToPayTax { get; set; }

        [Format(ClientFormat = "c", ApplyFormatInEditMode = true, DataFormatString = "c")]
        [DisplayName("Total Amount")]
        public Decimal? AmountToPayTotal { get; set; }

        public int IndexOfRow { get; set; }

    }

    public class InvoiceValidator : AbstractValidator<InvoiceViewModel>
    {
        public InvoiceValidator()
        {
            RuleFor(model => model.Date)
                .NotEmpty()
                .When(model => model.Number != null)
                .WithMessage("Invoice Date is required");

            RuleFor(model => model.Number)
                .NotEmpty()
                .When(model => model.Date != null)
                .WithMessage("Invoice Number is required");

            RuleFor(model => model.Number)
                .GreaterThan(0)
                .When(model => model.Date != null)
                .WithMessage("The Invoice Number must be greater than 0");

            RuleFor(model => model.AmountToPayGross)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(model => model.AmountGross);

            RuleFor(model => model.AmountToPayTax)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(model => model.AmountTax);

            RuleFor(model => model.AmountToPayTotal)
                .GreaterThan(0);
        }
    }
}