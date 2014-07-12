using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Attributes;

namespace WS.App.VAPSInventory.ViewModels
{
    [Validator(typeof(AddMonthValidator))]
    public class AddMonthViewModel
    {
        public int? LocationNumber { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }
    }

    public class AddMonthValidator : AbstractValidator<AddMonthViewModel>
    {
        public AddMonthValidator()
        {
            RuleFor(model => model.LocationNumber)
                .NotEmpty()
                .WithMessage("Branch is required");

            RuleFor(model => model.Year)
                .NotEmpty()
                .WithMessage("Year is required");

            RuleFor(model => model.Month)
                .NotEmpty()
                .WithMessage("Month is required");
        }
    }
}