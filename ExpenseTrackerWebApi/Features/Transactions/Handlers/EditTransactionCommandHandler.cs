using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Tags.Models;
using ExpenseTrackerWebApi.Features.Transactions.Commands;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.SharedKernel.Transactions.Handlers
{
    public class EditTransactionCommandHandler : IRequestHandler<EditTransactionCommand, int>
    {
        private readonly AppDbContext _context;
        public EditTransactionCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditTransactionCommand request, CancellationToken cancellationToken)
        {
            var oldTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

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

            if(oldTransactions == null)
                throw new ArgumentException("Transaction not found!");

            if(oldTransactions.Category.IdentityUserId != request.UserId)
                throw new UnauthorizedAccessException("Transaction does not belong to user!");

            if (!userCategoryIds.Contains((int)request.TransactionDto.CategoryId!))
                throw new UnauthorizedAccessException("Category does not belong to user");

            if (!userAccountIds.Contains((int)request.TransactionDto.AccountId!))
                throw new UnauthorizedAccessException("Account does not belong to user");

            if (!request.TransactionDto.TagIds!.All(id => tags.Contains(id)))
                throw new UnauthorizedAccessException("Tag does not belong to user");

            oldTransactions.Amount = (decimal)request.TransactionDto.Amount!;
            oldTransactions.Description = request.TransactionDto.Description;
            oldTransactions.Date = request.TransactionDto.Date!.Value.Add(request.TransactionDto.Time!.Value);
            oldTransactions.AccountId = (int)request.TransactionDto.AccountId!;
            oldTransactions.CategoryId = (int)request.TransactionDto.CategoryId!;
            oldTransactions.IsReoccuring = request.TransactionDto.IsReoccuring;
            oldTransactions.ReoccuranceFrequency = (ReoccuranceFrequency?)request.TransactionDto.ReoccuranceFrequency;
            
            if(oldTransactions.IsReoccuring!= null && oldTransactions.IsReoccuring == true && oldTransactions.ReoccuranceFrequency != null){
                switch(oldTransactions.ReoccuranceFrequency){
                    case ReoccuranceFrequency.Daily:
                        oldTransactions.NextReoccuranceDate = oldTransactions.Date.AddDays(1);
                        break;
                    case ReoccuranceFrequency.Weekly:
                        oldTransactions.NextReoccuranceDate = oldTransactions.Date.AddDays(7);
                        break;
                    case ReoccuranceFrequency.Monthly:
                        oldTransactions.NextReoccuranceDate = oldTransactions.Date.AddMonths(1);
                        break;
                    case ReoccuranceFrequency.Yearly:
                        oldTransactions.NextReoccuranceDate = oldTransactions.Date.AddYears(1);
                        break;
                }
            }
            
            oldTransactions.Amount = Math.Abs(oldTransactions.Amount);
            if ((TransactionType)request.TransactionDto.TransactionType! == TransactionType.Expense)
            {
                oldTransactions.Amount = -oldTransactions.Amount;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await _context.TransactionTags
                .Where(tt => tt.TransactionId == oldTransactions.Id)
                .ExecuteDeleteAsync(cancellationToken);

            _context.ChangeTracker.Clear();

            var tt = new List<TransactionTag>();
            foreach (var tag in request.TransactionDto.TagIds)
            {
                tt.Add(new TransactionTag()
                {
                    TagId = tag,
                    TransactionId = request.Id
                });
            }
            _context.TransactionTags.AddRange(tt);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Id;
        }
    }
}