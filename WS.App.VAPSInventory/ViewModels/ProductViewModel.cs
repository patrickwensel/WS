using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Attributes;

namespace WS.App.VAPSInventory.ViewModels
{
    [Validator(typeof(ProductValidator))]
    public class ProductViewModel
    {
        public int ID { get; set; }
        public int ProductCategoryID { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductValidator : AbstractValidator<ProductViewModel>
    {
        public ProductValidator()
        {
            RuleFor(model => model.ProductCategoryID)
                .NotEmpty()
                .WithMessage("Please select a Product Category");

            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(1, 50);

            RuleFor(model => model.Description)
                .Length(0, 80);

        }
    }

}