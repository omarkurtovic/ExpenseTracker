namespace ExpenseTrackerSharedCL.Features.Auth.Dtos
{
    public class RegisterDataDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public RegisterDataDto()
        {
            Email = "";
            Password = "";
            PasswordConfirm = "";
        }
    }
}
