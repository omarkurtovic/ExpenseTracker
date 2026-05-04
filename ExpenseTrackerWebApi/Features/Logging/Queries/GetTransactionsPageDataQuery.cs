using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Logging.Queries
{
    public class GetLogsPageDataQuery : IRequest<LogsPageDataDto>
    {
        public required string UserId { get; set; }
        public required LogsGridOptionsDto LogOptions { get; set; }
    }



    public class GetLogsPageDataQueryValidator : AbstractValidator<GetLogsPageDataQuery>
    {
        public GetLogsPageDataQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.LogOptions)
                .NotNull().WithMessage("Log options are required!");
        }
    }
}