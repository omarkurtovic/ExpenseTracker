using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerWebApi.Features.Auth.Dtos
{
    public class LoginDataDto
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public bool RememberMe { get; set; } = false;
    }
}
