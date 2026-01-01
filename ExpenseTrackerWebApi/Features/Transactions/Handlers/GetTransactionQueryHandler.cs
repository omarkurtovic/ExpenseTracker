using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
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
            .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            

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
                ReoccuranceFrequency = (ReoccuranceFrequencyDto)transaction.ReoccuranceFrequency!,
                NextReoccuranceDate = transaction.NextReoccuranceDate,
                TagIds = transaction.TransactionTags.Select(tt => tt.TagId).ToList(),
                TransactionTags = [.. transaction.TransactionTags.Select(tt => new TransactionTagDto
                {
                    TagId = tt.TagId,
                    TransactionId = tt.TransactionId,
                    Tag = new TagDto()
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }
                })]
            };
        }
    }
}