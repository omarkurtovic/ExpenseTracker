using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Transactions.Handlers
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
    {
        private readonly AppDbContext _context;

        public GetTransactionsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransactionTags)
                .ThenInclude(tt => tt.Tag)
            .Where(t => t.Account.IdentityUserId == request.UserId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

            if (request.IsReoccuring)
            {
                result = result.Where(t => t.IsReoccuring!= null && t.IsReoccuring == true).ToList();
            }
            else
            {
                result = result.Where(t => t.IsReoccuring == null || t.IsReoccuring == false).ToList();
            }

            return [.. result.Select(transaction => new TransactionDto()
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                Time = transaction.Date.TimeOfDay,
                CategoryId = transaction.CategoryId,
                AccountId = transaction.AccountId,
                CategoryDto = new CategoryDto()
                {
                    Id = transaction.Category.Id,
                    Name = transaction.Category.Name,
                    Type = (TransactionTypeDto)transaction.Category.Type,
                    Color = transaction.Category.Color,
                    Icon = transaction.Category.Icon
                },
                AccountDto = new AccountDto()
                {
                    Id = transaction.Account.Id,
                    Name = transaction.Account.Name,
                    InitialBalance = transaction.Account.InitialBalance,
                    Icon = transaction.Account.Icon,
                    Color = transaction.Account.Color
                },
                TransactionType = (TransactionTypeDto)transaction.Category.Type,
                IsReoccuring = transaction.IsReoccuring,
                ReoccuranceFrequency = transaction.ReoccuranceFrequency != null ? (ReoccuranceFrequencyDto)transaction.ReoccuranceFrequency : null,
                NextReoccuranceDate = transaction.NextReoccuranceDate,
                TagIds = transaction.TransactionTags.Select(tt => tt.TagId).ToList(),
                TransactionTags = [.. transaction.TransactionTags.Select(tt => new TransactionTagDto()
                {
                    TagId = tt.TagId,
                    TransactionId = tt.TransactionId,
                })],
            })];
        }

    }
}