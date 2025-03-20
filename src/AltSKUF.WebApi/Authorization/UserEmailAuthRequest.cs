using System.ComponentModel.DataAnnotations;

namespace AltSKUF.WebApi.Authorization
{
    public class UserEmailAuthRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
