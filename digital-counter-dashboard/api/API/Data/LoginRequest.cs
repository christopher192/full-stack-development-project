using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;
    }
}
