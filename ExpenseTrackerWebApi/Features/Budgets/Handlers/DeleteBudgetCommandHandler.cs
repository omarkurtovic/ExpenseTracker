using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Budgets.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Budgets.Handlers
{
    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand>
    {        
        private readonly AppDbContext _context;

        public DeleteBudgetCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _context.Budgets
            .Where(b => b.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken); 

            if(budget == null)
            {
                throw new ArgumentException("Budget not found!");
            }

            if(budget.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Budget does not belong to user!");
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}