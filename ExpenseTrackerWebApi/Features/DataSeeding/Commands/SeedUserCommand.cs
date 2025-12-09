using ExpenseTrackerSharedCL.Features.DataSeeding.Dtos;
using FluentValidation;
using MediatR;
    
namespace ExpenseTrackerWebApi.Features.DataSeeding.Commands
{
    public class SeedUserCommand : IRequest
    {
        public required string UserId {get; set;}
        public required DataSeedOptionsDto Options {get; set;}
    }


    public class SeedUserQueryValidator : AbstractValidator<SeedUserCommand>
    {
        public SeedUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.Options)
                .NotNull().WithMessage("Seed data options are required!");

            RuleFor(x => x.Options.NumberOfTransaction)
                .NotNull().WithMessage("Number of transactions is required!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.NumberOfTransaction)
                .GreaterThan(0).WithMessage("Number of transactions must be greater than 0!")
                .When(x => x.Options != null && x.Options.NumberOfTransaction.HasValue);

            RuleFor(x => x.Options.NumberOfTransaction)
                .LessThan(10000).WithMessage("Number of transactions must be less than 10000!")
                .When(x => x.Options != null && x.Options.NumberOfTransaction.HasValue); 

            RuleFor(x => x.Options.TransactionMinAmount)
                .NotNull().WithMessage("Transaction minimum amount is required!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.TransactionMinAmount)
                .GreaterThan(0).WithMessage("Transaction minimum amount must be greater than 0!")
                .When(x => x.Options != null && x.Options.TransactionMinAmount.HasValue);

            RuleFor(x => x.Options.TransactionMaxAmount)
                .NotNull().WithMessage("Transaction maximum amount is required!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.TransactionMaxAmount)
                .GreaterThan(x => x.Options.TransactionMinAmount).WithMessage("Transaction maximum amount must be greater than minimum amount!")
                .When(x => x.Options != null && x.Options.TransactionMaxAmount.HasValue && x.Options.TransactionMinAmount.HasValue);

            RuleFor(x => x.Options.TransactionMaxAmount)
                .LessThan(1000000).WithMessage("Transaction maximum amount must be less than 1,000,000!")
                .When(x => x.Options != null && x.Options.TransactionMaxAmount.HasValue);
            
            RuleFor(x => x.Options.TransactionStartDate)
                .NotNull().WithMessage("Transaction start date is required!")
                .When(x => x.Options != null);
            
            RuleFor(x => x.Options.TransactionEndDate)
                .NotNull().WithMessage("Transaction end date is required!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.TransactionStartDate)
                .LessThan(x => x.Options.TransactionEndDate).WithMessage("Transaction start date must be before end date!")
                .When(x => x.Options != null && x.Options.TransactionStartDate.HasValue && x.Options.TransactionEndDate.HasValue);

            RuleFor(x => x.Options.MaxNumberOfTags)
                .NotNull().WithMessage("Maximum number of tags is required!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.MaxNumberOfTags)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum number of tags must be at least 0!")
                .When(x => x.Options != null);

            RuleFor(x => x.Options.MaxNumberOfTags)
                .LessThanOrEqualTo(10).WithMessage("Maximum number of tags must be at most 10!")
                .When(x => x.Options != null);
        }
    }

}