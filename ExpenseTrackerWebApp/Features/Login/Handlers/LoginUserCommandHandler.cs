using ExpenseTrackerWebApp.Features.Login.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Login.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, SignInResult>
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginUserCommandHandler(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<SignInResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: false
            );
        }
    }
}
