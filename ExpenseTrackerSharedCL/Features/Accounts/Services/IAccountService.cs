using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Accounts.Services
{
    public interface IAccountService
    {
        public Task<Result<List<AccountDto>>> GetAccountsAsync();

        public Task<Result<List<AccountWithBalanceDto>>> GetAccountsWithBalanceAsync();

        public Task<Result<AccountDto>> GetAccountAsync(int accountId);


        public Task<Result> CreateAccountAsync(AccountDto accountDto);


        public Task<Result> EditAccountAsync(AccountDto accountDto);

        public Task<Result> DeleteAccountAsync(int accountId);
    }
}
