// Features/_SharedKernel/Behaviors/ValidationBehavior.cs
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ExpenseTrackerWebApi.Features.SharedKernel.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            var context = new ValidationContext<TRequest>(request);
            var failures = new List<ValidationFailure>();

            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                failures.AddRange(result.Errors);
            }

            if (failures.Count != 0) 
                throw new ValidationException(failures);

            return await next(cancellationToken);
        }
    }
}