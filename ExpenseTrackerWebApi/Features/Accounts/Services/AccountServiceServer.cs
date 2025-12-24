using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Accounts.Services;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.Accounts.Services
{
    public class AccountServiceServer : IAccountService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<Result> CreateAccountAsync(AccountDto accountDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAccountAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> EditAccountAsync(AccountDto accountDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<AccountDto>> GetAccountAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<AccountDto>>> GetAccountsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var accounts = await _mediator.Send(new GetAccountsQuery() { UserId = userId });
                return Result<List<AccountDto>>.Success(accounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accounts: {ex.Message}");
                return Result<List<AccountDto>>.Failure("An error occurred while fetching accounts.");
            }
        }

        public async Task<Result<List<AccountWithBalanceDto>>> GetAccountsWithBalanceAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var accounts = await _mediator.Send(new GetAccountsWithBalanceQuery() { UserId = userId });
                return Result<List<AccountWithBalanceDto>>.Success(accounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accounts: {ex.Message}");
                return Result<List<AccountWithBalanceDto>>.Failure("An error occurred while fetching accounts.");
            }
        }
    }
}
