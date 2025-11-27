using ExpenseTrackerWebApp.Features.Login.Commands;
using ExpenseTrackerWebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ExpenseTrackerWebApp.Features.Login.Controllers
{
    
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ISender _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(ISender mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            var result = await _mediator.Send(new LoginUserCommand
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            });
            
            if (result.Succeeded)
            {
                await _mediator.Send(new CheckForReoccuringTransactionsCommand
                {
                    UserId = _currentUserService.GetUserId()
                });
                return Redirect("/");
            }
            
            return Redirect($"/login?error=true&email={email}");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutUserCommand());
            return Redirect("/login");
        }
    }
}
