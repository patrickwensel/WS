using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Attributes;

namespace WS.App.VAPSInventory.ViewModels
{
    [Validator(typeof(ProductCategoryValidator))]
    public class ProductCategoryViewModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductCategoryValidator : AbstractValidator<ProductCategoryViewModel>
    {
        public ProductCategoryValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(1, 50);

            RuleFor(model => model.Description)
                .Length(0, 80);

        }
    }
}