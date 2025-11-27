using FluentValidation;
using ExpenseTrackerWebApp.Features.SharedKernel.Dtos;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : CommandResult, new()
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (ValidationException vex)
            {
                var response = new TResponse();
                response.Success = false;
                response.Errors = vex.Errors.Select(e => e.ErrorMessage).ToList();
                return response;
            }
            catch (Exception ex)
            {
                var response = new TResponse();
                response.Success = false;
                response.Errors = new() { ex.Message };
                return response;
            }
        }
    }
}
