namespace ExpenseTrackerSharedCL.Features.Login.Dtos
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe{get; set;}
    }
}