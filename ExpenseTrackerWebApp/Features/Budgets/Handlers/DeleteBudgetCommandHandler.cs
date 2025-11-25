using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
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
            await _context.Budgets
            .Where(b => b.Id == request.Id)
            .Where(b => b.IdentityUserId == request.UserId)
            .ExecuteDeleteAsync();
        }
    }

}