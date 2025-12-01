using ExpenseTrackerWebApp.Features.Register.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Register.Handlers
{
    public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand, Unit>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISender _mediator;

        public RegisterNewUserCommandHandler(UserManager<IdentityUser> userManager, ISender mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(RegisterNewUserCommand request, CancellationToken cancellationToken)
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
            return Unit.Value;
        }
    }
}