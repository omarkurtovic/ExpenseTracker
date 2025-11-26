using ExpenseTrackerWebApp.Features.Categories.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public required string UserId{get; set;}
        public required CategoryDto CategoryDto{get; set;}
    }

     public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.CategoryDto)
                .NotEmpty().WithMessage("Category is required!");

            RuleFor(x => x.CategoryDto.Name)
                .NotEmpty().WithMessage("Name is required!");

            RuleFor(x => x.CategoryDto.Type)
                .NotNull().WithMessage("Type is required!");

            RuleFor(x => x.CategoryDto.Icon)
                .NotEmpty().WithMessage("Icon is required!");

            RuleFor(x => x.CategoryDto.Color)
                .NotEmpty().WithMessage("Icon color is required!");
        }
    }
}