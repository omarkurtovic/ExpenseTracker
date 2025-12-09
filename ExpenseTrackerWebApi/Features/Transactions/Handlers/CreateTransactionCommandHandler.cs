using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Transactions.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.SharedKernel.Transactions.Handlers
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
    {
        private readonly AppDbContext _context;
        public CreateTransactionCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var userCategoryIds = await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var userAccountIds = await _context.Accounts
                .Where(a => a.IdentityUserId == request.UserId)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            var tags = await _context.Tags
                .Where(a => a.IdentityUserId == request.UserId)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            if (!userCategoryIds.Contains((int)request.TransactionDto.CategoryId!))
                throw new UnauthorizedAccessException("Category does not belong to user");

            if (!userAccountIds.Contains((int)request.TransactionDto.AccountId!))
                throw new UnauthorizedAccessException("Account does not belong to user");

            if (!request.TransactionDto.TagIds!.All(id => tags.Contains(id)))
                throw new UnauthorizedAccessException("Tag does not belong to user");

            var transaction = new Transaction()
            {
                Amount = (decimal)request.TransactionDto.Amount!,
                Description = request.TransactionDto.Description,
                Date = request.TransactionDto.Date!.Value.Add(request.TransactionDto.Time!.Value),
                AccountId = (int)request.TransactionDto.AccountId!,
                CategoryId = (int)request.TransactionDto.CategoryId!,
                IsReoccuring = request.TransactionDto.IsReoccuring,
                ReoccuranceFrequency = (ReoccuranceFrequency?)request.TransactionDto.ReoccuranceFrequency
            };

            if(transaction.IsReoccuring!= null && transaction.IsReoccuring == true && transaction.ReoccuranceFrequency != null){
                switch(transaction.ReoccuranceFrequency){
                    case ReoccuranceFrequency.Daily:
                        transaction.NextReoccuranceDate = transaction.Date.AddDays(1);
                        break;
                    case ReoccuranceFrequency.Weekly:
                        transaction.NextReoccuranceDate = transaction.Date.AddDays(7);
                        break;
                    case ReoccuranceFrequency.Monthly:
                        transaction.NextReoccuranceDate = transaction.Date.AddMonths(1);
                        break;
                    case ReoccuranceFrequency.Yearly:
                        transaction.NextReoccuranceDate = transaction.Date.AddYears(1);
                        break;
                }
            }
            
            transaction.Amount = Math.Abs(transaction.Amount);
            if ((TransactionType)request.TransactionDto.TransactionType! == TransactionType.Expense)
            {
                transaction.Amount = -transaction.Amount;
            }


            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            var tt = new List<TransactionTag>();
            foreach (var tag in request.TransactionDto.TagIds)
            {
                tt.Add(new TransactionTag()
                {
                    TagId = tag,
                    TransactionId = transaction.Id
                });
            }
            _context.TransactionTags.AddRange(tt);
            await _context.SaveChangesAsync(cancellationToken);
            return transaction.Id;
        }
    }
}