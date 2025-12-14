using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.Models
{
    public class LoginRequest
    {
        [Required]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
