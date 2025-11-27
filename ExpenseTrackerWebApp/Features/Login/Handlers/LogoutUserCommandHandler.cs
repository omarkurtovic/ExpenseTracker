using ExpenseTrackerWebApp.Features.Login.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Login.Handlers
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutUserCommandHandler(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }
    }
}
