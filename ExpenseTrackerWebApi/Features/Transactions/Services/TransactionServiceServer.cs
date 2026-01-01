using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Services;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using System.Security.Claims;
using System.Transactions;

namespace ExpenseTrackerWebApi.Features.Transactions.Services
{
    public class TransactionServiceServer : ITransactionService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransactionServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<Result> CreateTransactionAsync(TransactionDto transactionDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteTransactionAsync(int transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<TransactionsPageDataDto>> GetTransactionsAsync(TransactionsGridOptionsDto options)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var transactions = await _mediator.Send(new GetTransactionsPageDataQuery() { UserId = userId, TransactionOptions = options });
                return Result<TransactionsPageDataDto>.Success(transactions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transactions: {ex.Message}");
                return Result<TransactionsPageDataDto>.Failure("An error occurred while fetching transactions.");

            }
        }

        public Task<Result> UpdateTransactionAsync(TransactionDto transactionDto)
        {
            throw new NotImplementedException();
        }
    }
}
