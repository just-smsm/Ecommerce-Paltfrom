using System.ComponentModel.DataAnnotations;
namespace Ecommerce_platforms.API.ModelsDTO
{
    public class RegisterUserDTO
    {
        public string FirstName   { get; set; }
        [Required] 
        public string LastName { get;set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
