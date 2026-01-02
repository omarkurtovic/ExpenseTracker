using System.Globalization;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Enums;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;

namespace ExpenseTrackerWebApi.Features.Transactions.Handlers
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
    {
        private readonly AppDbContext _context;

        public GetAllTransactionsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactionsQuery = _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransactionTags)
                .ThenInclude(tt => tt.Tag)
            .Where(t => t.Account.IdentityUserId == request.UserId);
            if(request.IsReoccuring)
            {
                transactionsQuery = transactionsQuery.Where(t => t.IsReoccuring == true);
            }
            else
            {
                transactionsQuery = transactionsQuery.Where(t => t.IsReoccuring == null || t.IsReoccuring == false);
            }
            transactionsQuery = transactionsQuery.OrderByDescending(t => t.Date);
            
            return [.. (await transactionsQuery.ToListAsync(cancellationToken)).Select(transaction => new TransactionDto()
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
                    Tag = new TagDto()
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }
                })],
            })];
        }

    }
}