using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Dtos;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Handlers
{
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto>
    {
        private readonly AppDbContext _context;
        public GetTransactionQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            Transaction? transaction = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Include(t => t.TransactionTags)
                .ThenInclude(tt => tt.Tag)
            .Where(b => b.Id == request.Id).FirstOrDefaultAsync();
            

            if(transaction == null)
            {
                throw new ArgumentException("Transaction not found!");
            }

            if(transaction.Category.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Transaction does not belong to user!");
            }

            return new TransactionDto
            {
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                Time = transaction.Date.TimeOfDay,
                CategoryId = transaction.CategoryId,
                AccountId = transaction.AccountId,
                TransactionType = transaction.Category.Type,
                IsReoccuring = transaction.IsReoccuring,
                ReoccuranceFrequency = transaction.ReoccuranceFrequency,
                TagIds = transaction.TransactionTags.Select(tt => tt.TagId).ToList()
            };
        }
    }
}