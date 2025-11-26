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
                if(request.CategoryDto.Type == TransactionType.Income)
                {
                    await _context.Transactions
                    .Where(t => t.CategoryId == request.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.Amount, t => t.Amount < 0 ? -t.Amount : t.Amount));
                }
                else if(request.CategoryDto.Type == TransactionType.Expense)
                {
                    await _context.Transactions
                    .Where(t => t.CategoryId == request.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.Amount, t => t.Amount > 0 ? -t.Amount : t.Amount));
                }
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