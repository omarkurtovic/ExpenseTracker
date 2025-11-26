using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Categories.Handlers
{
    public class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, int>
    {
        private readonly AppDbContext _context;

        public EditCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
        {
            var oldCategory = await _context.Categories
                .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if(oldCategory == null)
            {
                throw new ArgumentException("Category not found!");
            }

            if(oldCategory.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Category does not belong to user!");
            }

            if(oldCategory.Type != (TransactionType)request.CategoryDto.Type)
            {
                var transactions = await _context.Transactions
                    .Where(t => t.CategoryId == request.Id)
                    .ToListAsync(cancellationToken);

                foreach (var tx in transactions)
                {
                    if(request.CategoryDto.Type == TransactionType.Income)
                    {
                        tx.Amount = Math.Abs(tx.Amount);
                    }
                    else if(request.CategoryDto.Type == TransactionType.Expense)
                    {
                        tx.Amount = -Math.Abs(tx.Amount);
                    }
                }
                
                await _context.SaveChangesAsync(cancellationToken);
            }
            oldCategory.Name = request.CategoryDto.Name!;
            oldCategory.Type = (TransactionType)request.CategoryDto.Type!;
            oldCategory.Icon = request.CategoryDto.Icon;
            oldCategory.Color = request.CategoryDto.Color;
            await _context.SaveChangesAsync(cancellationToken);


            return oldCategory.Id;
        }
    }
}