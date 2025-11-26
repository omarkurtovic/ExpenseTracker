using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Accounts.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers{
    public class EditAccountCommandHandler : IRequestHandler<EditAccountCommand, int>
    {
        private readonly AppDbContext _context;
        public EditAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(EditAccountCommand request, CancellationToken cancellationToken)
        {
            var oldAccount = await _context.Accounts
                .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if(oldAccount == null)
            {
                throw new ArgumentException("Account not found!");
            }

            if(oldAccount.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Account does not belong to user!");
            }


            oldAccount.Name = request.AccountDto.Name!;
            oldAccount.InitialBalance = (decimal)request.AccountDto.InitialBalance!;
            oldAccount.Icon = request.AccountDto.Icon;
            oldAccount.Color = request.AccountDto.Color;
            await _context.SaveChangesAsync(cancellationToken);
            return oldAccount.Id;
        }
    }
}