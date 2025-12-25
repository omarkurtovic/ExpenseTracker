using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Transactions.Services
{
    public interface ITransactionService
    {
        public Task<Result<TransactionsPageDataDto>> GetTransactionsAsync(TransactionsGridOptionsDto options);

        public Task<Result> DeleteTransactionAsync(int transactionId);

        public Task<Result> CreateTransactionAsync(TransactionDto transactionDto);

        public Task<Result> UpdateTransactionAsync(TransactionDto transactionDto);
    }
}
