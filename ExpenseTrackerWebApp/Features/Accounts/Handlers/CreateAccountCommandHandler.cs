using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Accounts.Commands;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers{
    
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account()
            {
                Name = request.AccountDto.Name!,
                InitialBalance = (decimal)request.AccountDto.InitialBalance!,
                IdentityUserId = request.UserId,
                Icon = request.AccountDto.Icon,
                Color = request.AccountDto.Color
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);
            return account.Id;
        }
    }
    
}