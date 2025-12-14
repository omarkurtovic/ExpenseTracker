using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTrackerSharedCL.Features.Auth.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Commands;
using ExpenseTrackerWebApi.Features.Auth.Commands;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.UserPreferences.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTrackerWebApi.Features.Auth.Controllers
{

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
        private readonly ISender _mediator;   

    public AuthController(UserManager<IdentityUser> userManager, ISender mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var token = GenerateJwt(user);
        await _mediator.Send(new CheckForReoccuringTransactionsCommand() { UserId = user.Id });
        return Ok(new LoginResponseDto(){Token = token});
    }

    [HttpPost]  
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDataDto request)
    {
        var user = new IdentityUser 
        { 
            UserName = request.Email, 
            Email = request.Email 
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        if(!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _mediator.Send(new ResetToDefaultCategoriesCommand() { UserId = user.Id });
        await _mediator.Send(new ResetToDefaultAccountsCommand() { UserId = user.Id });
        await _mediator.Send(new AddDefaultUserPreferencesCommand() { UserId = user.Id });
        return Ok();
    }

    private static string GenerateJwt(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "ExpenseTrackerWebApi",
            audience: "ExpenseTrackerWebApiUsers",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
}