using System.ComponentModel.DataAnnotations;

namespace Ecommerce_platforms.API.ModelsDTO
{
    public class LoginDTo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
