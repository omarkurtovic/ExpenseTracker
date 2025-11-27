using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Register.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;

namespace ExpenseTrackerWebApp.Features.Register.Handlers
{
    public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand>
    {
        private readonly AppDbContext _context;
        private readonly ISender _mediator;
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterNewUserCommandHandler(AppDbContext context, ISender mediator, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
        {
            var user = new IdentityUser 
            { 
                UserName = request.RegisterDataDto.Email, 
                Email = request.RegisterDataDto.Email 
            };
            
            var result = await _userManager.CreateAsync(user, request.RegisterDataDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("User registration failed");
            }

            await _mediator.Send(new ResetToDefaultCategoriesCommand { UserId = user.Id });
            await _mediator.Send(new AddDefaultUserPreferencesCommand { UserId = user.Id });
            await _mediator.Send(new ResetToDefaultAccountsCommand { UserId = user.Id });
        }
    }
}