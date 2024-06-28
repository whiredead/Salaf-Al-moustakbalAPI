using System.ComponentModel.DataAnnotations;

namespace SalafAlmoustakbalAPI.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
