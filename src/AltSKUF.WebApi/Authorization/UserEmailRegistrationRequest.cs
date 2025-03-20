using System.ComponentModel.DataAnnotations;

namespace AltSKUF.WebApi.Authorization
{
    public class UserEmailRegistrationRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Role { get; set; }
    }
}
